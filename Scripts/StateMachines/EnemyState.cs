using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


public class EnemyState : BaseState<AIState>
{
    public override AIState Process(Character character)
    {
        return AIState.Patrol;
    }

    public override AIState CheckState(Character character)
    {
        if (character.Stats.UnitType == UnitType.Human)
        {
            // Emergency conditions first
            if (character.Stats.Health.GetValue() <= 2)
                return AIState.Flee;
                
            if (character.Stats.Morale.GetValue() < 20)
                return AIState.Cower;

            // Combat decisions
            if (PlayerInSight(character))
                return AIState.Tactical;
        }
        else if (character.Stats.UnitType == UnitType.Alien && PlayerInSight(character))
        {
            return AIState.Aggression;
        }

        // Stay in current state if no condition is met
        return AIState.Patrol;
    }
}