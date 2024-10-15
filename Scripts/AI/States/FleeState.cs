using Godot;

public class FleeState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Flee State");
    }

    public override AIState Process(Enemy enemy)
    {
        GD.Print("Fleeing!");
        return AIState.Flee;
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Flee State");
    }
}