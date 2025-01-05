using System.Threading.Tasks;
using Godot;
using System.Linq;
using System;

public class AggressionState : EnemyState
{
    public override void Enter(Character enemy)
    {
        base.Enter(enemy);
        enemy.CharacterController.IsEnemyAlerted = true;

        if (enemy.enemiesInLos.Count > 0)
            enemy.Target = enemy.enemiesInLos[0];
    }

    public override async Task Decide(Character enemy)
    {
        var nextState = CheckState(enemy);
        if (nextState != enemy.enemyController._stateMachine.CurrentState)
        {
            enemy.enemyController.SetState(nextState, enemy);
            return;
        }
        await enemy.enemyController.HandleAggression();
        nextState = CheckState(enemy);
        if (nextState != enemy.enemyController._stateMachine.CurrentState)
        {
            enemy.enemyController.SetState(nextState, enemy);
            return;
        }
    }

    public override void Exit(Character enemy)
    {
        enemy.Target = null;
    }
}