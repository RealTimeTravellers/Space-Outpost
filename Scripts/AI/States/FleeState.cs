using Godot;

public class FleeState : BaseState
{
    public override void Enter(Character aiController)
    {
        GD.Print("Entering Flee State");
    }

    public override AIState Process(Character enemy)
    {
        GD.Print("Fleeing!");
        return AIState.Flee;
    }

    public override void Exit(Character aiController)
    {
        GD.Print("Exiting Flee State");
    }

    public override AIState CheckState(Character enemy)
    {
        if (!PlayerInSight(enemy))
        {
            return AIState.Tactical;
        }
        return AIState.Flee;
    }
}