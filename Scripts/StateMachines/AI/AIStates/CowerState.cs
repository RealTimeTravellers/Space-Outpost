using Godot;
using System.Threading.Tasks;
using System;
public class CowerState : EnemyState
{
    public override void Enter(Character enemy)
    {
        base.Enter(enemy);
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
        await enemy.enemyController.HandleCower();
    }


    public override void Exit(Character enemy)
    {
        base.Exit(enemy);
    }
}