using Godot;

public class CowerState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print("Entering Cower State");
    }

    public override AIState Process(Character enemy)
    {
        GD.Print("In Fear!");
        return AIState.Cower;
    }

    public override void Exit(Character aiController)
    {
        GD.Print("Exiting Cower State");
    }

    public override AIState CheckState(Character enemy)
    {
        if (EnemyInSight(enemy))
        {
            return AIState.Tactical;
        }
        return AIState.Cower;
    }
}