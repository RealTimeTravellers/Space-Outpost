using Godot;

public partial class Enemy : Node3D
{
    private EnemyAIController _aiController;
    public UnitStats Stats { get; private set; }

    public override void _Ready()
    {
        _aiController = new EnemyAIController();
        Stats = new UnitStats();
    }

    public void SetState(AIState newState)
    {
        _aiController.SetState(newState, this);
    }

    public void TakeTurn()
    {
        _aiController.UpdateAI(this);
    }
}
