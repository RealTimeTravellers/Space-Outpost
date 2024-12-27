using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class EnemyState : BaseState<AIState>
{
    bool canSeePlayer = false;

    public override AIState Process(Character character)
    {
        if (character.CompletedTurn || character.IsMoving)
            return character.enemyController._stateMachine.CurrentState;
            
        return CheckState(character);
    }
    

    public override AIState CheckState(Character character)
    {
        canSeePlayer = character.enemiesInLos.Count > 0;

        // Düşük can durumunda Flee
        if (character.Stats.Health.GetValue() <= 2 || character.Health <= 2)
            return AIState.Flee;

        // State değişimlerini kontrol et
        if (character.Stats.UnitType == UnitType.Alien)
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

        if (!canSeePlayer && (character.enemyController._stateMachine.CurrentState == AIState.Alert || 
            character.enemyController._stateMachine.CurrentState == AIState.Aggression || 
            character.enemyController._stateMachine.CurrentState == AIState.Tactical))
        {
            return AIState.Patrol;
        }

        return character.enemyController._stateMachine.CurrentState;
    }

    
}