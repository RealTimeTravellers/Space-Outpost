using Godot;
public class TacticalState : EnemyState
{
    private GridObject _targetCover;

    public override void Enter(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Entering Tactical State");
        FindClosestTarget(enemy);
        if (enemy.Target != null)
        {
            _targetCover = FindNearestCover(enemy, enemy.Target);
        }
    }

    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Tactical)
            return nextState;

        if (enemy.Target == null || enemy.Target.Stats.Health.GetValue() <= 0)
        {
            FindClosestTarget(enemy);
            if (enemy.Target == null)
                return AIState.Patrol;
                
            _targetCover = FindNearestCover(enemy, enemy.Target);
        }

        // Cover'a gitme ve savaşma mantığı
        if (!enemy.IsInCover && _targetCover != null)
        {
            enemy.Move(_targetCover);
            enemy.TakeCover();
            enemy.CompletedTurn = true;
            TurnManager.Instance.EndEnemyMovement(enemy);
        }
        else if (enemy.IsInCover)
        {
            if (enemy.Equipment.CurrentWeapon.NeedsReload())
            {
                enemy.Equipment.CurrentWeapon.Reload();
                enemy.CompletedTurn = true;
                TurnManager.Instance.EndEnemyMovement(enemy);
            }
            else if (enemy.Stats.ActionPoints.GetValue() >= 2)
            {
                enemy.Attack(enemy.Target);
                enemy.CompletedTurn = true;
                TurnManager.Instance.EndEnemyMovement(enemy);
            }
            else
            {
                enemy.CompletedTurn = true;
                TurnManager.Instance.EndEnemyMovement(enemy);
            }
        }
        
        return AIState.Tactical;
    }

    public override void Exit(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Exiting Tactical State");
        _targetCover = null;
    }
}