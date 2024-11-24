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
    [Export] public Godot.Collections.Array<Character> playerCharacters = new();
    //[Export] public Godot.Collections.Dictionary<Character, bool> playerCharacterTurns = new();

    private TurnManager()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        SetInitialTurn();
        PlayerMovementChanged += OnPlayerMovementChanged;
        base._Ready();
    }

    private async void SetInitialTurn()
    {
        await ToSignal(Instance.GetTree(), SceneTree.SignalName.ProcessFrame);

        TurnChanged.Invoke(true);
    }

    public void StartEnemyMovement()
    {
        EnemyMovementChanged.Invoke(true);
    }

    public void EndEnemyMovement()
    {
        EnemyMovementChanged.Invoke(false);
    }

    public void StartPlayerMovement()
    {
        PlayerMovementChanged.Invoke(true);
    }

    public void EndPlayerMovement()
    {
        PlayerMovementChanged.Invoke(false);
    }

    private void OnPlayerMovementChanged(bool started)
    {
        if(started) return;

        if (!started) // player turn finished
        {
            bool allCompletedTurns = true;

            foreach (Character player in playerCharacters)
                allCompletedTurns |= player.CompletedTurn;

            if (allCompletedTurns)
                TurnChanged.Invoke(false); // start enemy turn
        }

    }
}
