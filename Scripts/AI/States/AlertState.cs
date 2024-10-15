using Godot;

public class AlertState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Alert State");
    }

    public override AIState Process(Enemy enemy)
    {
        GD.Print("Alert! Searching for player...");
        return CheckState(enemy);
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Alert State");
    }

    public override AIState CheckState(Enemy enemy)
    {
        if (enemy.Stats.unitType == UnitType.Alien && PlayerInSight(enemy))
        {
            return AIState.Aggression;
        }
        else if (enemy.Stats.unitType == UnitType.Human && PlayerInSight(enemy))
        {
            return AIState.Tactical;
        }
        else if (enemy.Stats.unitType == UnitType.Human && enemy.Stats.Morale.GetValue() < 20)
        {
            return AIState.Cower;
        }
        else if (enemy.Stats.unitType == UnitType.Human && enemy.Stats.Health.GetValue() <= 2)
        {
            return AIState.Flee;
        }
        return AIState.Alert;
    }
}