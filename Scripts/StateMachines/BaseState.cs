using System;
using Godot;

public abstract class BaseState<TStateType> : IBaseState<TStateType> where TStateType : Enum
{
    protected TStateType PreviousState { get; private set; }
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

        var enemiesInSight = enemy.QueryForEnemies(TurnManager.Instance.playerCharacters);
        return enemiesInSight.Count > 0;
    }

    protected bool EnemyInSight(Character character)
    {
        if (TurnManager.Instance == null) return false;

        var enemies = character.IsFriendly ? 
            TurnManager.Instance.enemyCharacters : 
            TurnManager.Instance.playerCharacters;

        var enemiesInSight = character.QueryForEnemies(enemies);
        return enemiesInSight.Count > 0;
    }

    protected void FindClosestTarget(Character enemy)
    {
        var enemiesInSight = enemy.QueryForEnemies(TurnManager.Instance.playerCharacters);
        if (enemiesInSight.Count > 0)
        {
            enemy.Target = enemiesInSight[0]; // İlk düşmanı hedef al
            GD.Print($"[AI] {enemy.Name} found target: {enemy.Target?.Name}");
        }
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