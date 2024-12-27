using Godot;

public class CowerState : EnemyState
{
    private bool _isHandlingCower = false;

    public override void Enter(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Entering Cower State");
        aiController.CharacterController.IsEnemyAlerted = true;
    }

    public override AIState Process(Character enemy)
    {
        if (enemy.enemyController._turnPlayed) return AIState.Cower;
        
        var baseState = base.Process(enemy);
        if (baseState != AIState.Cower)
            return baseState;

        if (enemy.CompletedTurn)
            return AIState.Cower;

        if (!_isHandlingCower)
        {
            _isHandlingCower = true;
            enemy.enemyController._turnPlayed = true;
            enemy.enemyController.HandleCower().ContinueWith(_ => {
                _isHandlingCower = false;
            });
        }

        return AIState.Cower;
    }

    public override void Exit(Character aiController)
    {
        GD.Print($"[AI] {aiController.Name} Exiting Cower State");
        _isHandlingCower = false;
    }
}