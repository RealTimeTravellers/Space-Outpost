using Godot;

public class TacticalState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Entering Tactical State");
    }

    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Tactical)
            return nextState;

        // Taktiksel davranış
        if (!enemy.IsInCover)
        {
            enemy.TakeCover();
            return AIState.Tactical;
        }

        if (enemy.Equipment.CurrentWeapon.NeedsReload())
        {
            enemy.Equipment.CurrentWeapon.Reload();
            return AIState.Tactical;
        }

        if (enemy.Target != null && enemy.Stats.ActionPoints.GetValue() >= 2)
        {
            enemy.Attack(enemy.Target);
        }
        
        return AIState.Tactical;
    }
    public override void Exit(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Exiting Tactical State");
    }
}