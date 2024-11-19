using Godot;

public class PatrolState : BaseState
{
    public override void Enter(Character enemy)
    {
        GD.Print("Entering Patrol State");
    }

    public override AIState Process(Character enemy)
    {
        GD.Print("Patrolling!");
        return CheckState(enemy);
    }

    public override void Exit(Character enemy)
    {
        GD.Print("Exiting Patrol State");
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
        return AIState.Patrol;
    }
}
