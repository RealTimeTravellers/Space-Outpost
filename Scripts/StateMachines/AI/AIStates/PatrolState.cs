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
        
        if (_patrolTarget == null)
        {
            ChooseNewDirection(enemy);
            enemy.enemyController.MoveToGrid(_patrolTarget);
        }

        return base.Process(enemy);
    }

    public override void Exit(Character enemy)
    {
        _patrolTarget = null;
    }
}