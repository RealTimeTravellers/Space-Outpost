using Godot;
using System.Linq;
public class PatrolState : EnemyState
{
    private GridObject _patrolTarget;
    private bool _isMoving = false;

    public override void Enter(Character enemy)
    {
        enemy.CharacterController.IsEnemyAlerted = false;
        _isMoving = false;
        _patrolTarget = enemy.enemyController.HandlePatrol(enemy);
    }

    public override AIState Process(Character enemy)
    {       
        if (enemy.enemyController._turnPlayed) return AIState.Patrol;

        var baseState = base.Process(enemy);
        if (baseState != AIState.Patrol)
            return baseState;

        if (enemy.CompletedTurn)
            return AIState.Patrol;

        if (_patrolTarget != null && !_isMoving)
        {
            GD.Print($"[AI Debug] {enemy.Name} moving to patrol target");
            _isMoving = true;
            enemy.enemyController._turnPlayed = true;
            enemy.Move(_patrolTarget).ContinueWith(_ => 
            {
                GD.Print($"[AI Debug] {enemy.Name} movement completed");
                _isMoving = false;
                _patrolTarget = null;
            });
        }
        
        return AIState.Patrol;
    }

    public override void Exit(Character enemy)
    {
        _patrolTarget = null;
        _isMoving = false;
    }
}