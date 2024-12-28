using Godot;
using System.Threading.Tasks;
public class TacticalState : EnemyState
{
    private GridObject _targetCover;

    public override void Enter(Character enemy)
    {
        base.Enter(enemy);
        enemy.CharacterController.IsEnemyAlerted = true;
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
        await enemy.enemyController.HandleTactical();
    }

    public override void Exit(Character enemy)
    {
        _targetCover = null;
        base.Exit(enemy);
    }
}