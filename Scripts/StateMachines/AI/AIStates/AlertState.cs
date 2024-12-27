using Godot;
public class AlertState : EnemyState
{
    private bool _isHandlingAlert = false;

    public override void Enter(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Entering Alert State");
        enemy.CharacterController.IsEnemyAlerted = true;

    }

    public override AIState Process(Character enemy)
    {
        if (enemy.enemyController._turnPlayed) return AIState.Alert;
        
        var baseState = base.Process(enemy);
        if (baseState != AIState.Alert)
            return baseState;

        if (enemy.CompletedTurn)
            return AIState.Alert;

        if (!_isHandlingAlert && !enemy.IsMoving)
        {
            _isHandlingAlert = true;
            enemy.enemyController._turnPlayed = true;
            enemy.enemyController.HandleAlert().ContinueWith(_ => {
                _isHandlingAlert = false;
            });
        }

        return AIState.Alert;
    }

    public override void Exit(Character enemy)
    {
        _isHandlingAlert = false;
    }
}