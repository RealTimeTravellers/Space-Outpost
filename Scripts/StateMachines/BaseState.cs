using System;
using Godot;

public abstract class BaseState<TStateType> : IBaseState<TStateType> where TStateType : Enum
{
    public virtual void Enter(Character character)
    {
        
    }

    public virtual TStateType Process(Character character)
    {
        return default;
    }

    public virtual void Exit(Character character)
    {
        
    }

    public virtual TStateType CheckState(Character character)
    {
        return default;
    }

    protected bool PlayerInSight(Character enemy)
    {
        if (TurnManager.Instance == null) return false;

        foreach (Character player in TurnManager.Instance.playerCharacters)
        {
            if (player == null || player == enemy) continue;  // Skip self-check

            float distance = enemy.GlobalPosition.DistanceTo(player.GlobalPosition);
            GD.Print($"[AI] {enemy.Name} checking distance to {player.Name}: {distance}, Perception range: {enemy.Stats.Perception.GetValue()}");
            
            if (distance <= enemy.Stats.Perception.GetValue())
            {
                var space = enemy.GetWorld3D().DirectSpaceState;
                var query = PhysicsRayQueryParameters3D.Create(
                    enemy.GlobalPosition + Vector3.Up, 
                    player.GlobalPosition + Vector3.Up
                );
                query.CollisionMask = 1 << 1;
                var result = space.IntersectRay(query);

                if (result.Count == 0)
                    return true;
            }
        }
        return false;
    }

    protected bool EnemyInSight(Character character)
    {
        if (TurnManager.Instance == null) return false;

        var enemies = character.IsFriendly ? 
            TurnManager.Instance.enemyCharacters : 
            TurnManager.Instance.playerCharacters;

        foreach (Character potentialTarget in enemies)
        {
            if (potentialTarget == null || potentialTarget.Stats.Health.GetValue() <= 0)
                continue;

            // First check distance
            float distance = character.GlobalPosition.DistanceTo(potentialTarget.GlobalPosition);
            if (distance <= character.Stats.Perception.GetValue())
            {
                // Then do line of sight check
                var space = character.GetWorld3D().DirectSpaceState;
                var query = PhysicsRayQueryParameters3D.Create(
                    character.GlobalPosition + Vector3.Up, 
                    potentialTarget.GlobalPosition + Vector3.Up
                );
                query.CollisionMask = 1 << 1; // Layer 1 (walls)
                var result = space.IntersectRay(query);

                if (result.Count == 0) // No wall in between
                    return true;
            }
        }
        
        return false;
    }
}