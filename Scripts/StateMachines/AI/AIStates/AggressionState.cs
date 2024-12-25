using System.Linq;
using Godot;

public class AggressionState : EnemyState
{
    private const float MAX_MOVE_DISTANCE = 10f;

    public override void Enter(Character enemy)
    {
        enemy.Target = enemy.enemiesInLos[0];
        float distanceToTarget = enemy.GlobalPosition.DistanceTo(enemy.Target.GlobalPosition);

        if (distanceToTarget > enemy.Stats.Perception.GetValue())
        {
            var grid = GridManager.Instance.GetGridObjectFromWorldPosition(enemy.Target.GlobalPosition);
            enemy.CharacterController._stateMachine.ChangeState(CharacterStateType.Moving, enemy);
            enemy.enemyController.MoveToGrid(grid);
        }
        else if (enemy.Stats.ActionPoints.GetValue() >= 2)
        {
            enemy.CharacterController._stateMachine.ChangeState(CharacterStateType.Aiming, enemy);
            enemy.Attack(enemy.Target);
            enemy.CharacterController._stateMachine.ChangeState(CharacterStateType.Idle, enemy);
        }

        enemy.CompletedTurn = true;
        TurnManager.Instance.EndEnemyMovement(enemy);
    }

    public override AIState Process(Character enemy)
    {
        return base.Process(enemy);
    }

    public override void Exit(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Exiting Aggression State");
        enemy.Target = null;
    }
}