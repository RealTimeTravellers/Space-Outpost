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
        CurrentlyMovingCharacter.IsMoving = true;
        PrintAllCharacterStatus("StartEnemyMovement");
        EnemyMovementChanged?.Invoke(true);
    }

    public void EndEnemyMovement(Character character)
    {
        CurrentlyMovingCharacter = character;
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
        PrintAllCharacterStatus("EndPlayerMovement");
        PlayerMovementChanged?.Invoke(false);
    }

    private void OnPlayerMovementChanged(bool started)
    {
        if (started)
        {
            GD.Print("[TurnManager] Player movement started");
            return;
        }

        GD.Print("[TurnManager] Player movement ended");

        bool allCompletedTurns = playerCharacters
            .Where(e => e != null && !e.IsDead)
            .All(e => e.CompletedTurn);

        GD.Print($"[TurnManager] All players completed turns: {allCompletedTurns}");

        if (allCompletedTurns && !_isProcessingTurn)
        {
            GD.Print("[TurnManager] All players completed, switching to enemy turn");
            _isProcessingTurn = true;
            EndPlayerTurn();
            TurnChanged.Invoke(false);
            _isProcessingTurn = false;
        }
    }

    private void OnEnemyMovementChanged(bool started)
    {
        if (started)
        {
            GD.Print("[TurnManager] Enemy movement started");
            CurrentlyMovingCharacter.IsMoving = true;
            return;
        }

        GD.Print("[TurnManager] Enemy movement ended");
        CurrentlyMovingCharacter.IsMoving = false;

        if (_isProcessingTurn) return;

        bool allEnemiesCompleted = enemyCharacters
            .Where(e => e != null && !e.IsDead)
            .All(e => e.CompletedTurn);

        if (allEnemiesCompleted && !_isProcessingTurn)
        {
            GD.Print("[TurnManager] All enemies completed, switching to player turn");
            _isProcessingTurn = true;
            EndEnemyTurn();
            TurnChanged.Invoke(true);
            _isProcessingTurn = false;
        }
    }

    private void EndPlayerTurn()
    {
        foreach (Character enemy in enemyCharacters.Where(e => e != null && !e.IsDead))
        {
            enemy.CompletedTurn = false;
            enemy.ResetActionPoints();
        }

        foreach (Character player in playerCharacters.Where(p => p != null && !p.IsDead))
        {
            player.CompletedTurn = false;
            player.DepleteActionPoints();
        }
        
        GD.Print("[TurnManager] Player turn ended");
    }

    private void EndEnemyTurn()
    {
        foreach (Character enemy in enemyCharacters.Where(e => e != null && !e.IsDead))
        {
            enemy.CompletedTurn = false;
            enemy.DepleteActionPoints();
        }

        foreach (Character player in playerCharacters.Where(e => e != null && !e.IsDead))
        {
            player.CompletedTurn = false;
            player.ResetActionPoints();
        }

        GD.Print("[TurnManager] Enemy turn ended");
    }

    public void PrintAllCharacterStatus(string context)
    {
        GD.Print($"\n[Turn Debug] {context} - Character Status Report:");
        GD.Print("--- Player Characters ---");
        foreach (var player in playerCharacters)
        {
            if (player != null)
            {
                GD.Print($"{player.Name}: CompletedTurn={player.CompletedTurn}, ActionPoints={player.Stats.ActionPoints.GetValue()}, IsDead={player.IsDead}");
            }
        }
        
        GD.Print("--- Enemy Characters ---");
        foreach (var enemy in enemyCharacters)
        {
            if (enemy != null)
            {
                GD.Print($"{enemy.Name}: CompletedTurn={enemy.CompletedTurn}, ActionPoints={enemy.Stats.ActionPoints.GetValue()}, IsDead={enemy.IsDead}");
            }
        }
        GD.Print("------------------------\n");
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
