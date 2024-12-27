using System.Threading.Tasks;
using Godot;

public class FleeState : EnemyState
{
    private bool _isHandlingFlee = false;

    public override void Enter(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Entering Flee State");
        enemy.CharacterController.IsEnemyAlerted = true;
    }

    public override AIState Process(Character enemy)
    {
        if (enemy.enemyController._turnPlayed) return AIState.Flee;

        var baseState = base.Process(enemy);
        if (baseState != AIState.Flee)
            return baseState;

        if (enemy.CompletedTurn)
            return AIState.Flee;

        if (!_isHandlingFlee && !enemy.IsMoving && !enemy.enemyController._turnPlayed)
        {
            _isHandlingFlee = true;
            enemy.enemyController._turnPlayed = true;
            enemy.enemyController.HandleFlee().ContinueWith(_ => {
                _isHandlingFlee = false;
            });
        }

        return AIState.Flee;
    }

    public override void Exit(Character enemy)
    {
        _isHandlingFlee = false;
    }
}