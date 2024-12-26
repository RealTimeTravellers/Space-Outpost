using Godot;

public class AlertState : EnemyState
{
    private bool _isMoving = false;
    public override async void Enter(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Entering Alert State");
        enemy.CharacterController.IsEnemyAlerted = true;

        if (EnemyManager.Instance.LastShotGrid != null)
        {
            _isMoving = true;
            await enemy.enemyController.MoveToGrid(EnemyManager.Instance.LastShotGrid);
            _isMoving = false;
            
            // Hareket tamamlandığında
            enemy.CompletedTurn = true;
            TurnManager.Instance.EndEnemyMovement(enemy);
        }
    }

    public override AIState Process(Character enemy)
    {
        return base.Process(enemy);
    }

    public override void Exit(Character enemy)
    {

    }
}