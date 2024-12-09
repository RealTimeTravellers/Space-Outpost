using Godot;

public class AlertState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Entering Alert State");
    }

    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Alert)
            return nextState;
            
        // Alert durumunda çevreyi tara
        if (!enemy.IsInCover)
            enemy.TakeCover();
            
        // Mühimmat kontrolü
        if (enemy.Equipment.CurrentWeapon.NeedsReload())
            enemy.Equipment.CurrentWeapon.Reload();
            
        return AIState.Alert;
    }

    public override void Exit(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Exiting Alert State");
    }
}