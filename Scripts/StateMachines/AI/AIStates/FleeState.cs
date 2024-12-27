using System.Threading.Tasks;
using Godot;

public class FleeState : EnemyState
{
    public override void Enter(Character enemy)
    {
        base.Enter(enemy);
        GD.Print($"[AI] {enemy.Name} Entering Flee State");
        enemy.CharacterController.IsEnemyAlerted = true;
    }

    public override async Task Decide(Character enemy)
    {
        var nextState = CheckState(enemy);
        if (nextState != enemy.enemyController._stateMachine.CurrentState)
        {
            enemy.enemyController.SetState(nextState, enemy);
            return;
        }
        await enemy.enemyController.HandleFlee();
    }

    public override void Exit(Character enemy)
    {
        base.Exit(enemy);
    }
}