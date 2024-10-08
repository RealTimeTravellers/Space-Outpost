using Godot;

public partial class Enemy : Node3D
{
    private EnemyAIController _aiController;

    public override void _Ready()
    {
        _aiController = new EnemyAIController();
    }

    public override void _Process(double delta)
    {
        _aiController.UpdateAI(this, (float)delta);
    }

    public void SetState(AIState newState)
    {
        _aiController.SetState(newState, this);
    }
}
