using Godot;
using System;
using System.Threading;

public partial class TurnManager : Node
{
    public static TurnManager Instance {get; private set;}

    /// <summary>
    /// Invokes True if player turn
    /// </summary>
    public event Action<bool> TurnChanged;

    public event Action EnemyTurnStarted;
    public event Action EnemyTurnEnded;

    private TurnManager()
    {
        Instance = this;
    }

    private async void StartPlayerTurn()
    {
        await ToSignal(Instance.GetTree(), SceneTree.SignalName.ProcessFrame);

        TurnChanged.Invoke(true);
    }
}
