using Godot;

public class CowerState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Entering Cower State");
    }

    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Cower)
            return nextState;

        // Sinmiş durumda bekle
        if (!enemy.IsInCover)
        {
            enemy.TakeCover();
        }
        
        // Moral yükseldiyse Alert state'e geç
        if (enemy.Stats.Morale.GetValue() >= 20)
            return AIState.Alert;
            
        return AIState.Cower;
    }

    public override void Exit(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Exiting Cower State");
    }
}