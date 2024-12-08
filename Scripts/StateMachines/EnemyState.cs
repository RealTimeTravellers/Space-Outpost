using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class EnemyState : BaseState<AIState>
{
    protected readonly Vector3[] directions = new[]
    {
        new Vector3(1, 0, 0),   // Sağ
        new Vector3(-1, 0, 0),  // Sol
        new Vector3(0, 0, 1),   // İleri
        new Vector3(0, 0, -1)   // Geri
    };
    public override AIState Process(Character character)
    {
        return AIState.Patrol;
    }

    public override AIState CheckState(Character character)
    {
        // Önce Alien kontrolü yap
        if (character.Stats.UnitType == UnitType.Alien)
        {
            if (PlayerInSight(character))
                return AIState.Aggression;
            return AIState.Patrol;  // Varsayılan duruma dön
        }

        // Human için kontroller
        if (character.Stats.Health.GetValue() <= 2)
            return AIState.Flee;
                
        if (character.Stats.Morale.GetValue() < 5)
            return AIState.Cower;

        if (PlayerInSight(character))
            return AIState.Tactical;

        return AIState.Patrol;  // Varsayılan duruma dön
    }
}