using Godot;

public class AlertState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print("Entering Alert State");
    }

    public override AIState Process(Character enemy)
    {
        GD.Print("Alert! Searching for player...");
        return CheckState(enemy);
    }

    public override void Exit(Character aiController)
    {
        GD.Print("Exiting Alert State");
    }

    public override AIState CheckState(Character enemy)
    {
        if (enemy.Stats.UnitType == UnitType.Alien && PlayerInSight(enemy))
        {
            return AIState.Aggression;
        }
        else if (enemy.Stats.UnitType == UnitType.Human && PlayerInSight(enemy))
        {
            return AIState.Tactical;
        }
        else if (enemy.Stats.UnitType == UnitType.Human && enemy.Stats.Morale.GetValue() < 20)
        {
            return AIState.Cower;
        }
        else if (enemy.Stats.UnitType == UnitType.Human && enemy.Stats.Health.GetValue() <= 2)
        {
            return AIState.Flee;
        }
        return AIState.Alert;
    }
}