using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class EnemyState : BaseState<AIState>
{
    bool canSeePlayer = false;

    public override void Enter(Character character)
    {
        base.Enter(character);
    }

    public override AIState Process(Character character)
    {
        return AIState.Patrol;
    }

    public override Task Decide(Character character)
    {
        return Task.CompletedTask;
    }

    public override void Exit(Character character)
    {
        base.Exit(character);
    }
    

    public override AIState CheckState(Character character)
    {
        // character.SearchForEnemies();
        canSeePlayer = character.enemiesInLos.Count > 0;

        // Düşük can durumunda Flee
        if (character.Health/* Stats.Health.GetValue() */ <= 2 || character.Health <= 2)
            return AIState.Flee;

        // State değişimlerini kontrol et
        if (character./* Stats. */UnitType == UnitType.Alien)
        {
            if (canSeePlayer)
                return AIState.Aggression;
        }
        else if (canSeePlayer)
        {
            return AIState.Tactical;
        }
        else if (EnemyManager.Instance.ShotFired)
        {
            return AIState.Alert;
        }

        return character.enemyController._stateMachine.CurrentState;
    }
}