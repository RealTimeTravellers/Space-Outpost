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

    protected void FindClosestTarget(Character enemy)
    {
        float closestDistance = float.MaxValue;
        Character closestTarget = null;

        foreach (Character player in TurnManager.Instance.playerCharacters)
        {
            if (player == null || player.Stats.Health.GetValue() <= 0) continue;

            float distance = enemy.GlobalPosition.DistanceTo(player.GlobalPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = player;
            }
        }

        enemy.Target = closestTarget;
        GD.Print($"[AI] {enemy.Name} found target: {closestTarget?.Name}");
    }

    protected GridObject FindNearestCover(Character enemy, Character target)
    {
        float maxDistance = enemy.Stats.Perception.GetValue();
        float bestScore = float.MinValue;
        GridObject bestCover = null;

        // Karakterin etrafındaki grid'leri kontrol et
        for (int x = -3; x <= 3; x++)
        {
            for (int z = -3; z <= 3; z++)
            {
                Vector3 checkPos = enemy.GlobalPosition + new Vector3(x * 2, 0, z * 2);
                var grid = GridManager.Instance.GetGridObjectFromWorldPosition(checkPos);
                
                if (grid == null || grid.IsOccupied || grid.IsBlocked || grid.coverType == CoverType.None) 
                    continue;

                float distanceToGrid = enemy.GlobalPosition.DistanceTo(checkPos);
                if (distanceToGrid > maxDistance) continue;

                // Cover'ın hedefle olan açısını kontrol et
                Vector3 coverToTarget = (target.GlobalPosition - checkPos).Normalized();
                Vector3 coverNormal = grid.CoverNormal;
                float coverAngle = Mathf.Abs(coverToTarget.Dot(coverNormal));

                // Puanlama: Cover tipi, açı ve mesafeye göre
                float coverScore = grid.coverType == CoverType.Full ? 2f : 1f;
                float score = (coverScore * coverAngle) - (distanceToGrid / maxDistance);
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCover = grid;
                }
            }
        }

        return bestCover;
    }
}