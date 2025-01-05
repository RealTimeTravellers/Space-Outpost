using Godot;
using System.Threading.Tasks;
public class TacticalState : EnemyState
{

    public override void Enter(Character enemy)
    {
        base.Enter(enemy);
        enemy.CharacterController.IsEnemyAlerted = true;
        //enemy.Target = enemy.enemiesInLos[0];
    }

    public override async Task Decide(Character enemy)
    {
        await enemy.enemyController.HandleTactical();
    }

    public override AIState CheckState(Character enemy)
    {
        return AIState.Tactical;
    }

    public override void Exit(Character enemy)
    {
        base.Exit(enemy);
    }
}