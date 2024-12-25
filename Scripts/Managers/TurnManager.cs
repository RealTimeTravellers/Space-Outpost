using Godot;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;

public partial class TurnManager : Node
{
    public static TurnManager Instance {get; private set;}

    /// <summary>
    /// Invokes True if player turn
    /// </summary>
    public event Action<bool> TurnChanged;

    /// <summary>
    /// if started True, false when finished
    /// </summary>
    public event Action<bool> EnemyMovementChanged;

    /// <summary>
    /// if started True, Flase when finished
    /// </summary>
    public event Action<bool> PlayerMovementChanged;

    /// <summary>
    /// True if completed. false if not completed
    /// </summary>
    
    public static Character CurrentlyMovingCharacter { get; private set; } = null;
    private bool _isProcessingTurn = false;
    private bool _isEnemyMoving = false;
    private int currentEnemyIndex = -1;

    public Action<Character> CharacterDied;
    [Export] public Godot.Collections.Array<Character> playerCharacters = new();
    [Export] public Godot.Collections.Array<Character> enemyCharacters = new();

    

    private TurnManager()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        SetInitialTurn();
        Instance.PlayerMovementChanged += Instance.OnPlayerMovementChanged;
        Instance.EnemyMovementChanged += Instance.OnEnemyMovementChanged;
        base._Ready();
    }

    public override void _ExitTree()
    {
        Instance.PlayerMovementChanged -= Instance.OnPlayerMovementChanged;
        Instance.EnemyMovementChanged -= Instance.OnEnemyMovementChanged;
        base._ExitTree();
    }

    private async void SetInitialTurn()
    {
        await ToSignal(Instance.GetTree(), SceneTree.SignalName.ProcessFrame);

        TurnChanged.Invoke(true);
    }

    public void StartEnemyMovement(Character character)
    {
        GD.Print("[TurnManager] Starting enemy movement");
        CurrentlyMovingCharacter = character;
        _isEnemyMoving = true;
        EnemyMovementChanged?.Invoke(true);
    }

    public void EndEnemyMovement(Character character)
    {
        GD.Print("[TurnManager] Ending enemy movement");
        CurrentlyMovingCharacter = character;
        _isEnemyMoving = false;
        EnemyMovementChanged?.Invoke(false);
    }

    public void StartPlayerMovement(Character character)
    {
        GD.Print("[TurnManager] Starting player movement");
        CurrentlyMovingCharacter = character;
        PlayerMovementChanged?.Invoke(true);
    }

    public void EndPlayerMovement(Character character)
    {
        GD.Print("[TurnManager] Ending player movement");
        CurrentlyMovingCharacter = character;
        PlayerMovementChanged?.Invoke(false);
    }

    private void OnPlayerMovementChanged(bool started)
    {
        GD.Print($"[TurnManager] Player Movement Changed: {started}");
        CheckEnemyState();
        if (started)
        {
            if (!_isProcessingTurn)
            {
                _isProcessingTurn = true;
                // Oyuncu turu başlarken düşmanların durumunu sıfırla
                foreach (Character enemy in enemyCharacters)
                {
                    enemy.CompletedTurn = false;
                }
                GD.Print("[TurnManager] Starting player turn processing");
            }
            return;
        }

        bool allCompletedTurns = true;
        GD.Print("[TurnManager] Checking all player turns:");
        foreach (Character player in playerCharacters)
        {
            GD.Print($"[TurnManager] - {player.Name}: {player.CompletedTurn}");
            allCompletedTurns &= player.CompletedTurn;
        }
        
        if (allCompletedTurns)
        {
            GD.Print("[TurnManager] All players completed, switching to enemy turn");
            _isProcessingTurn = false;
            TurnChanged?.Invoke(false);
            EndPlayerTurn();
            //StartEnemyMovement();
        }
    }

    private async void OnEnemyMovementChanged(bool started)
    {
        CheckEnemyState();
        GD.Print($"[TurnManager] Enemy Movement Changed: {started}");
        
        if (started)
        {
            _isEnemyMoving = true;
            return;
        }

        _isEnemyMoving = false;

        // Tüm düşmanlar tamamlandıysa oyuncu turuna geç
        await WaitForAllEnemiesCompleted();

        _isProcessingTurn = false;
        TurnChanged?.Invoke(true);   // player turn başlat
        EndEnemyTurn();             // düşman turn’unu kapat
    }

    private async Task WaitForAllEnemiesCompleted()
    {
        // Her 0.2s bir "hepsi bitti mi?" kontrolü
        while (true)
        {
            bool allEnemiesCompleted = enemyCharacters.All(e => e == null || e.CompletedTurn);
            if (allEnemiesCompleted)
                break;

            // 0.2 saniye bekle, sonra tekrar dene
            await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        }
    }

    private void EndPlayerTurn()
    {
        foreach (Character player in playerCharacters)
        {
            //player.CompletedTurn = true;
            player.Stats.DepleteActionPoints();
        }
        
        foreach (Character enemy in enemyCharacters)
        {
            enemy.CompletedTurn = false;
            enemy.Stats.ResetActionPoints();
            var aiController = enemy.GetNode<EnemyAIController>("EnemyAIController");
            if (aiController != null)
            {
                aiController._isActive = true;  // Tüm düşmanları aktif et
            }
        }
    }

    private void EndEnemyTurn()
    {
        foreach (Character enemy in enemyCharacters)
        {
            if (enemy != null)
            {
                enemy.CompletedTurn = true;
                enemy.Stats.DepleteActionPoints();
                var aiController = enemy.GetNode<EnemyAIController>("EnemyAIController");
                if (aiController != null)
                {
                    aiController._isActive = false;
                }
            }
        }

        foreach (Character player in playerCharacters)
        {
            //player.CompletedTurn = false;
            player.Stats.ResetActionPoints();
        }
    }

    private void CheckEnemyState()
    {
        foreach (Character enemy in enemyCharacters)
        {
            if (enemy == null || enemy.CompletedTurn) continue;
            
            var aiController = enemy.GetNode<EnemyAIController>("EnemyAIController");
            if (aiController != null)
            {
                var currentState = aiController._stateMachine._states[aiController._stateMachine.CurrentState];
                var nextState = currentState.CheckState(enemy);
                
                if (nextState != aiController._stateMachine.CurrentState)
                {
                    aiController.SetState(nextState, enemy);
                }
            }
        }
    }

    public void RemovePlayerCharacter(Character character)
    {
        if (playerCharacters.Contains(character))
            playerCharacters.Remove(character);
    }

    public void RemoveEnemyCharacter(Character character)
    {
        if (enemyCharacters.Contains(character))
            enemyCharacters.Remove(character);
    }
}
