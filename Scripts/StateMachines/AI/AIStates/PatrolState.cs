using Godot;
using System.Threading.Tasks;
public class PatrolState : EnemyState
{

    public override void Enter(Character enemy)
    {
        base.Enter(enemy);
        enemy.CharacterController.IsEnemyAlerted = false;
    }

    public override async Task Decide(Character enemy)
    {
        
        var nextState = CheckState(enemy);
        if (nextState != enemy.enemyController._stateMachine.CurrentState)
        {
            enemy.enemyController.SetState(nextState, enemy);
            return;
        }
        
        await enemy.enemyController.HandlePatrol(enemy);

        nextState = CheckState(enemy);
        if (nextState != enemy.enemyController._stateMachine.CurrentState)
        {
            enemy.enemyController.SetState(nextState, enemy);
            return;
        }
    }

    public override void Exit(Character enemy)
    {
        base.Exit(enemy);
    }
}