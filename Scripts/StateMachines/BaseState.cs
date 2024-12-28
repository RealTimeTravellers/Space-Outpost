using System;
using System.Threading.Tasks;
using Godot;
using System.Linq;
public abstract class BaseState<TStateType> : IBaseState<TStateType> where TStateType : Enum
{
    protected TStateType PreviousState { get; private set; }
    public virtual void Enter(Character character)
    {
        
    }

    public virtual Task Decide(Character character)
    {
        return Task.CompletedTask;
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

    protected bool EnemyInSight(Character character)
    {
        if (TurnManager.Instance == null || character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death) return false;
        
        var enemies = character.QueryForEnemies(new Godot.Collections.Array<Character>(
            character.IsFriendly ? 
            TurnManager.Instance.enemyCharacters : 
            TurnManager.Instance.playerCharacters
        ));
        
        return enemies.Count > 0;
    }

    protected void FindClosestTarget(Character enemy)
    {
        var enemiesInSight = enemy.QueryForEnemies(new Godot.Collections.Array<Character>
        (TurnManager.Instance.playerCharacters.Where(e => e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death)));
        if (enemiesInSight.Count > 0)
        {
            enemy.Target = enemiesInSight[0];
        }
    }

    /*
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
    */
}