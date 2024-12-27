using Godot;
public class TacticalState : EnemyState
{
    private GridObject _targetCover;
    private bool _isHandlingTactical = false;

    public override void Enter(Character enemy)
    {
        enemy.CharacterController.IsEnemyAlerted = true;
        enemy.Target = enemy.enemiesInLos[0];
    }

    public override AIState Process(Character enemy)
    {
        if (enemy.enemyController._turnPlayed) return AIState.Tactical;
        
        var baseState = base.Process(enemy);
        if (baseState != AIState.Tactical)
            return baseState;

        if (enemy.CompletedTurn)
            return AIState.Tactical;

        if (!_isHandlingTactical)
        {
            _isHandlingTactical = true;
            enemy.enemyController._turnPlayed = true;
            enemy.enemyController.HandleTactical().ContinueWith(_ => {
                _isHandlingTactical = false;
            });
        }

        return AIState.Tactical;
    }

    public override void Exit(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Exiting Tactical State");
        _targetCover = null;
        _isHandlingTactical = false;
    }
}