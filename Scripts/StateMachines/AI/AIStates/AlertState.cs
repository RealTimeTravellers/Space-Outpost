using Godot;
using System.Threading.Tasks;

public class AlertState : EnemyState
{

    public override void Enter(Character enemy)
    {
        base.Enter(enemy);
        GD.Print($"[AI] {enemy.Name} Entering Alert State");
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
        await enemy.enemyController.HandleAlert();
    }


    public override void Exit(Character enemy)
    {
        base.Exit(enemy);
    }
}