using Godot;

public class TacticalState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Tactical State");
    }

    public override AIState Process(Enemy enemy)
    {
        GD.Print("Tactical Positioning!");
        return CheckState(enemy);
    }
    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Tactical State");
    }

    public override AIState CheckState(Enemy enemy)
    {
        if (enemy.Stats.UnitType == UnitType.Human && enemy.Stats.Morale.GetValue() < 20)
        {
            return AIState.Cower;
        }
        else if (enemy.Stats.UnitType == UnitType.Human && enemy.Stats.Health.GetValue() <= 2)
        {
            return AIState.Flee;
        }
        return AIState.Tactical;
    }
}