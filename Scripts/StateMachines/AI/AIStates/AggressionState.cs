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
        await enemy.enemyController.HandleAggression();
    }

    public override AIState CheckState(Character enemy)
    {
        return AIState.Aggression;
    }

    public override void Exit(Character enemy)
    {
        enemy.Target = null;
    }
}