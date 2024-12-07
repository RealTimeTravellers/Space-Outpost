using Godot;
using Godot.NativeInterop;
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

    public void StartEnemyMovement()
    {
        GD.Print("[TurnManager] Starting enemy movement");
        EnemyMovementChanged?.Invoke(true);
    }

    public void EndEnemyMovement()
    {
        GD.Print("[TurnManager] Ending enemy movement");
        EnemyMovementChanged?.Invoke(false);
    }

    public void StartPlayerMovement()
    {
        GD.Print("[TurnManager] Starting player movement");
        foreach (Character player in playerCharacters)
            player.Stats.ResetActionPoints();
        
        PlayerMovementChanged?.Invoke(true);
    }

    public void EndPlayerMovement()
    {
        GD.Print("[TurnManager] Ending player movement");
        foreach (Character enemy in enemyCharacters)
            enemy.Stats.ResetActionPoints();
        
        PlayerMovementChanged?.Invoke(false);
    }

    private void OnPlayerMovementChanged(bool started)
    {
        GD.Print($"[TurnManager] Player Movement Changed: {started}");
        
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
            
            // Düşman turu başlarken oyuncuların durumunu sıfırla
            foreach (Character player in playerCharacters)
            {
                player.CompletedTurn = false;
            }
            
            foreach (Character enemy in enemyCharacters)
            {
                enemy.CompletedTurn = false;
                enemy.Stats.ResetActionPoints();
            }
            
            StartEnemyMovement();
        }
    }

    private void OnEnemyMovementChanged(bool started)
    {
        GD.Print($"[TurnManager] Enemy Movement Changed: {started}");
        
        if (started) return;

        // Tüm düşmanların durumunu kontrol et
        bool allEnemiesCompleted = true;
        bool foundNextEnemy = false;
        
        foreach (Character enemy in enemyCharacters)
        {
            if (enemy == null) continue;
            
            GD.Print($"[TurnManager] - {enemy.Name}: {enemy.CompletedTurn}");
            allEnemiesCompleted &= enemy.CompletedTurn;
            
            // Henüz tamamlanmamış düşman bul
            if (!enemy.CompletedTurn && !foundNextEnemy)
            {
                var aiController = enemy.GetNode<EnemyAIController>("AIController");
                if (aiController != null)
                {
                    aiController._isActive = true;
                    foundNextEnemy = true;
                    GD.Print($"[TurnManager] Activating next enemy: {enemy.Name}");
                    // StartEnemyMovement(); // Bu satırı kaldırıyoruz
                    break;
                }
            }
        }
        
        // Tüm düşmanlar tamamlandıysa oyuncu turuna geç
        if (allEnemiesCompleted && !foundNextEnemy)
        {
            GD.Print("[TurnManager] All enemies completed, switching to player turn");
            _isProcessingTurn = false;
            TurnChanged?.Invoke(true);
            
            foreach (Character player in playerCharacters)
            {
                if (player != null)
                {
                    player.CompletedTurn = false;
                    player.Stats.ResetActionPoints();
                }
            }
            
            StartPlayerMovement();
        }
    }
}
