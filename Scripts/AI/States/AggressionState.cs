using Godot;

public class AggressionState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Aggression State");
    }

    public override AIState Process(Enemy enemy)
    {
        GD.Print("Finding the player!");
        return AIState.Aggression;
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Aggression State");
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
        return AIState.Aggression;
    }
}