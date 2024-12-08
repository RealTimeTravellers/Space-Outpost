using Godot;

public class FleeState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Entering Flee State");
    }

    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Flee)
            return nextState;

        // Kaçış davranışı
        if (enemy.Target != null)
        {
            // Hedeften uzaklaş
            Vector3 fleeDirection = (enemy.Position - enemy.Target.Position).Normalized();
            // Kaçış noktasına hareket et
            enemy.Move(null); // GridManager'dan en uygun kaçış noktası bulunmalı
        }
        
        return AIState.Flee;
    }

    public override void Exit(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Exiting Flee State");
    }
}