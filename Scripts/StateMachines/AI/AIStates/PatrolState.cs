using Godot;
using System.Linq;
public class PatrolState : EnemyState
{
    private GridObject _patrolTarget;
    
    public override void Enter(Character enemy)
    {
        ChooseNewDirection(enemy);
    }

    private void ChooseNewDirection(Character enemy)
    {
        var random = new RandomNumberGenerator();
        random.Randomize();
        var shuffledDirections = directions.OrderBy(x => random.Randf()).ToArray();

        foreach (var dir in shuffledDirections)
        {
            int steps = random.RandiRange(1, 2);
            Vector3 targetPos = enemy.GlobalPosition + dir * steps;

            var targetGrid = GridManager.Instance.GetGridObjectFromWorldPosition(targetPos);
            if (targetGrid != null && !targetGrid.IsOccupied && !targetGrid.IsBlocked)
            {
                _patrolTarget = targetGrid;
                return;
            }
        }
    }

    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Patrol) 
            return nextState;

        if (enemy.CompletedTurn)
            return nextState;
        
        if (_patrolTarget == null)
            ChooseNewDirection(enemy);
        
        // Hareket başlatılmamışsa başlat
        enemy.enemyController.MoveToGrid(_patrolTarget);

        nextState = base.CheckState(enemy);
        if (nextState != AIState.Patrol) 
            return nextState;

        return AIState.Patrol;
    }

    public override void Exit(Character enemy)
    {
        _patrolTarget = null;
    }
}