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
}