using Godot;

public class AggressionState : EnemyState
{
    public override void Enter(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Entering Aggression State");
        FindClosestTarget(enemy);
    }

    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Aggression)
            return nextState;

        if (enemy.Target == null || enemy.Target.Stats.Health.GetValue() <= 0)
        {
            FindClosestTarget(enemy);
            if (enemy.Target == null)
                return AIState.Patrol;
        }

        // Hedef menzilde mi?
        float distanceToTarget = enemy.GlobalPosition.DistanceTo(enemy.Target.GlobalPosition);
        if (distanceToTarget > enemy.Stats.Perception.GetValue())
        {
            // Hedefe doğru hareket et
            var direction = (enemy.Target.GlobalPosition - enemy.GlobalPosition).Normalized();
            var targetPos = enemy.GlobalPosition + direction * 2; // 2 birim ileri
            var targetGrid = GridManager.Instance.GetGridObjectFromWorldPosition(targetPos);
            
            if (targetGrid != null && !targetGrid.IsOccupied && !targetGrid.IsBlocked)
            {
                enemy.Move(targetGrid);
                enemy.CompletedTurn = true;
                TurnManager.Instance.EndEnemyMovement();
            }
        }
        else
        {
            // Menzilde ise saldır
            if (enemy.Equipment.CurrentWeapon.NeedsReload())
            {
                enemy.Equipment.CurrentWeapon.Reload();
                enemy.CompletedTurn = true;
                TurnManager.Instance.EndEnemyMovement();
            }
            else if (enemy.Stats.ActionPoints.GetValue() >= 2)
            {
                enemy.Attack(enemy.Target);
                enemy.CompletedTurn = true;
                TurnManager.Instance.EndEnemyMovement();
            }
            else
            {
                enemy.CompletedTurn = true;
                TurnManager.Instance.EndEnemyMovement();
            }
        }
        
        return AIState.Aggression;
    }

    public override void Exit(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Exiting Aggression State");
        enemy.Target = null;
    }
}