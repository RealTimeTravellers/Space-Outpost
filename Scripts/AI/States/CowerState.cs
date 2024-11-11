using Godot;

public class CowerState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Cower State");
    }

    public override AIState Process(Enemy enemy)
    {
        GD.Print("In Fear!");
        return AIState.Cower;
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Cower State");
    }

    public override AIState CheckState(Enemy enemy)
    {
        if (EnemyInSight(enemy))
        {
            return AIState.Tactical;
        }
        return AIState.Cower;
    }
}