using System.Threading.Tasks;
using Godot;

public class FleeState : EnemyState
{
    private bool _isMoving = false;
    private const int FLEE_DISTANCE = 10;

    public override async void Enter(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Entering Alert State");
        enemy.CharacterController.IsEnemyAlerted = true;

        if (enemy.Target != null)
        {
            Vector3 fleeDirection = (enemy.GlobalPosition - enemy.Target.GlobalPosition).Normalized();
            Vector3 targetPosition = enemy.GlobalPosition + fleeDirection * FLEE_DISTANCE;
            
            var escapeGrid = GridManager.Instance.GetGridObjectFromWorldPosition(targetPosition);

            _isMoving = true;
            await enemy.enemyController.MoveToGrid(escapeGrid, FLEE_DISTANCE);
        }
    }

    public override AIState Process(Character enemy)
    {
        return base.Process(enemy);
    }

    public override void Exit(Character enemy)
    {
        _isMoving = false;
    }
}