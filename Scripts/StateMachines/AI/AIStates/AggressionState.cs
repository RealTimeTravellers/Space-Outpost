using Godot;

public class AggressionState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print("Entering Aggression State");
    }

    public override AIState Process(Character enemy)
    {
        GD.Print("Finding the player!");
        return AIState.Aggression;
    }

    public override void Exit(Character aiController)
    {
        GD.Print("Exiting Aggression State");
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
        return AIState.Aggression;
    }
}