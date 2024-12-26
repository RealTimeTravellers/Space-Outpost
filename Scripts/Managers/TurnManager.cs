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
        CurrentlyMovingCharacter = character;
        _isEnemyMoving = true;
        EnemyMovementChanged?.Invoke(true);
    }

    public void EndEnemyMovement(Character character)
    {
        CurrentlyMovingCharacter = character;
        _isEnemyMoving = false;
        EnemyMovementChanged?.Invoke(false);
    }

    public void StartPlayerMovement(Character character)
    {
        CurrentlyMovingCharacter = character;
        PlayerMovementChanged?.Invoke(true);
    }

    public void EndPlayerMovement(Character character)
    {
        CurrentlyMovingCharacter = character;
        PlayerMovementChanged?.Invoke(false);
    }

    private void OnPlayerMovementChanged(bool started)
    {
        if (started)  return;

        bool allCompletedTurns = true;

        foreach (Character player in playerCharacters)
        {
            if (player == null || player.IsDead) continue;
            allCompletedTurns &= player.CompletedTurn;
        }
        
        if (allCompletedTurns)
        {
            GD.Print("[TurnManager] All players completed, switching to enemy turn");

            TurnChanged?.Invoke(false);
            EndPlayerTurn();
            _isProcessingTurn = true;
        }
    }

    private void OnEnemyMovementChanged(bool started)
    {
        if (started)
        {
            _isEnemyMoving = true;
            return;
        }

        bool allEnemiesCompleted = enemyCharacters
            .Where(e => e != null && !e.IsDead)
            .All(e => e.CompletedTurn);

        if (allEnemiesCompleted)
        {
            GD.Print("[TurnManager] All enemies completed, switching to player turn");

            TurnChanged?.Invoke(true);
            EndEnemyTurn();
            _isProcessingTurn = true;
            _isEnemyMoving = false;
            
        }
    }

    private void EndPlayerTurn()
    {
        foreach (Character enemy in enemyCharacters.Where(e => e != null && !e.IsDead))
        {
            enemy.CompletedTurn = false;
            enemy.Stats.ResetActionPoints();
            enemy.enemyController._isActive = true;
        }
        
        foreach (Character player in playerCharacters.Where(p => p != null && !p.IsDead))
        {
            player.Stats.DepleteActionPoints();
            player.CompletedTurn = true;
        }
        
        _isProcessingTurn = true;
        GD.Print("[TurnManager] Player turn ended");
    }

    private void EndEnemyTurn()
    {
        
        foreach (Character enemy in enemyCharacters.Where(e => e != null && !e.IsDead))
        {
            enemy.CompletedTurn = true;
            enemy.Stats.DepleteActionPoints();
            enemy.enemyController._isActive = false;
        }

        foreach (Character player in playerCharacters.Where(e => e != null && !e.IsDead))
        {
            player.CompletedTurn = false;
            player.Stats.ResetActionPoints();
        }

        _isProcessingTurn = true;
        GD.Print("[TurnManager] Enemy turn ended");
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
