using System.Linq;
using Godot;

public class AggressionState : EnemyState
{
    private const float MAX_MOVE_DISTANCE = 10f;
    private bool _isHandlingAggression = false;

    public override void Enter(Character enemy)
    {
        GD.Print($"[AI Debug] {enemy.Name} Entering Aggression State");
        enemy.CharacterController.IsEnemyAlerted = true;
        enemy.Target = enemy.enemiesInLos[0];
    }

    public override AIState Process(Character enemy)
    {
        if (enemy.enemyController._turnPlayed) return AIState.Aggression;
        
        var baseState = base.Process(enemy);
        if (baseState != AIState.Aggression)
            return baseState;

        if (enemy.CompletedTurn)
            return AIState.Aggression;

        if (!_isHandlingAggression && !enemy.enemyController._turnPlayed)
        {
            _isHandlingAggression = true;
            enemy.enemyController._turnPlayed = true;
            enemy.enemyController.HandleAggression().ContinueWith(_ => {
                _isHandlingAggression = false;
            });
        }

        return AIState.Aggression;
    }

    public override void Exit(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Exiting Aggression State");
        enemy.Target = null;
        _isHandlingAggression = false;
    }
}