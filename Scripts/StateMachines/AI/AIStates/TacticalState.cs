using Godot;

public class TacticalState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print("Entering Tactical State");
    }

    public override AIState Process(Character enemy)
    {
        GD.Print("Tactical Positioning!");
        return CheckState(enemy);
    }
    public override void Exit(Character aiController)
    {
        GD.Print("Exiting Tactical State");
    }

    public override AIState CheckState(Character enemy)
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