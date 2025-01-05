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
    /// Invokes True if player turn async
    /// </summary>
    public event Func<bool, Task> TurnChangedAsync;

    /// <summary>
    /// if started True, false when finished
    /// </summary>
    public event Action<bool> EnemyMovementChanged;

    /// <summary>
    /// if started True, Flase when finished
    /// </summary>
    public event Action<bool> PlayerMovementChanged;

    public static Character CurrentlyMovingCharacter { get; private set; } = null;
    public bool isGameOver = false;
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
        PlayerMovementChanged?.Invoke(false);
    }

    private void OnPlayerMovementChanged(bool started)
    {
        if (started)
        {
            GD.Print("[TurnManager] Player movement started");
            CurrentlyMovingCharacter.IsMoving = true;
            return;
        }

        GD.Print("[TurnManager] Player movement ended");

        bool allCompletedTurns = playerCharacters
            .Where(e => e != null && e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death)
            .All(e => e.CompletedTurn);

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
            .Where(e => e != null && e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death)
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

    private async void EndPlayerTurn()
    {
        await Task.Delay(100);
        foreach (Character enemy in enemyCharacters.Where(e => e != null && e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death))
        {
            enemy.CompletedTurn = false;
            enemy.ResetActionPoints();
        }

        foreach (Character player in playerCharacters.Where(p => p != null && p.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death))
        {
            player.CompletedTurn = false;
            player.DepleteActionPoints();
            MissionManager.Instance.AddCharacterLog(MissionManager.Instance.logTexts.TurnEndLog, false, player.Name);

        }

        if (TurnChangedAsync != null)
        {
            var tasks = TurnChangedAsync.GetInvocationList()
                .Cast<Func<bool, Task>>()
                .Select(handler => handler.Invoke(false));
            await Task.WhenAll(tasks);
        }

        MissionManager.Instance.RecordTurnComplete();
    }

    private async void EndEnemyTurn()
    {
        await Task.Delay(100);
        foreach (Character enemy in enemyCharacters.Where(e => e != null && e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death))
        {
            enemy.CompletedTurn = false;
            enemy.DepleteActionPoints();
            MissionManager.Instance.AddCharacterLog(MissionManager.Instance.logTexts.TurnEndLog, true, enemy.Name);
        }

        foreach (Character player in playerCharacters.Where(e => e != null && e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death))
        {
            player.CompletedTurn = false;
            player.ResetActionPoints();
        }
    }

    public void RemoveCharacterSafely(Character character)
    {
        // Önce current character'ı kontrol et
        if (CurrentlyMovingCharacter == character)
            CurrentlyMovingCharacter = null;
        
        if (_isProcessingTurn && currentEnemyIndex >= 0)
        {
            if (currentEnemyIndex >= enemyCharacters.Count)
                currentEnemyIndex = enemyCharacters.Count - 1;
        }

        if (enemyCharacters.Count <= 0)
            AudioManager.Instance.combatEnded = true;

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (isGameOver) return;

        GD.Print($"[TurnManager] CheckGameOver called. isGameOver: {isGameOver}");

        // Defeat check, all dead ?
        int deadPlayerCount = playerCharacters.Count(p => 
            p.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death);
        
        if (deadPlayerCount >= 3)
        {
            isGameOver = true;
            GameManager.Instance.LoadEndScreen(false);
            return;
        }

        // Victory check, all enemies dead ?
        int deadEnemyCount = enemyCharacters.Count(e => 
            e.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death);

        if (enemyCharacters.Count == deadEnemyCount)
        {
            GD.Print($"Dead enemies: {deadEnemyCount}, Total enemies: {enemyCharacters.Count}");
            isGameOver = true;
            GameManager.Instance.LoadEndScreen(true);
        }
    }
}
