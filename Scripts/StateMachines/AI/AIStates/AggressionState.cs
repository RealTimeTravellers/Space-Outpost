using Godot;

public class AggressionState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print("Entering Aggression State");
    }

    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Aggression)
            return nextState;

        if (enemy.Target != null)
        {
            if (enemy.Equipment.CurrentWeapon.NeedsReload())
                enemy.Equipment.CurrentWeapon.Reload();
            else
                enemy.Attack(enemy.Target);
        }
        
        return AIState.Aggression;
    }

    public override void Exit(Character aiController)
    {
        GD.Print("Exiting Aggression State");
    }
}