using Godot;

public class CowerState : EnemyState
{
    public override void Enter(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Entering Cower State");
        aiController.CharacterController.IsEnemyAlerted = true;
    }

    public override AIState Process(Character enemy)
    {
        return base.Process(enemy);
    }

    public override void Exit(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Exiting Cower State");
    }
}