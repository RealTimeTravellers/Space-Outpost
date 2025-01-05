using System.Threading.Tasks;
using Godot;

public class FleeState : EnemyState
{
    private bool _isRunningAway = false;
    public override void Enter(Character enemy)
    {
        base.Enter(enemy);
        enemy.CharacterController.IsEnemyAlerted = true;
    }

    public override async Task Decide(Character enemy)
    {
        var nextState = CheckState(enemy);
        if (nextState != enemy.enemyController._stateMachine.CurrentState)
        {
            enemy.enemyController.SetState(nextState, enemy);
            return;
        }

        if (!_isRunningAway)
        {
            await enemy.enemyController.HandleFlee();
            _isRunningAway = true;
        }

        nextState = CheckState(enemy);
        if (nextState != enemy.enemyController._stateMachine.CurrentState)
        {
            enemy.enemyController.SetState(nextState, enemy);
            return;
        }
    }

    public override AIState CheckState(Character character)
    {
        if (!_isRunningAway){
            return AIState.Flee;
        }
        else
        {
            if (character.Stats.UnitType == UnitType.Alien)
                return AIState.Aggression;
            else
                return AIState.Tactical;
        }
    }

    public override void Exit(Character enemy)
    {
        base.Exit(enemy);
    }
}