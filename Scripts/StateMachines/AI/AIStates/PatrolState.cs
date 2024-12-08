using Godot;
using System.Linq;

public class PatrolState : EnemyState
{
    private Vector3 _patrolTarget;
    public override void Enter(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Entering Patrol State");
        ChooseNewDirection(enemy);
    }

    private void ChooseNewDirection(Character enemy)
    {
        var currentAP = enemy.Stats.ActionPoints.GetValue();
        if (currentAP <= 0)
        {
            enemy.CompletedTurn = true;
            TurnManager.Instance.EndEnemyMovement();
            return;
        }

        var random = new RandomNumberGenerator();
        random.Randomize();
        var shuffledDirections = directions.OrderBy(x => random.Randf()).ToArray();

        bool foundValidTarget = false;

        // Çarpışma ortamı için DirectSpaceState al
        var space = enemy.GetWorld3D().DirectSpaceState;

        // Tüm yönleri sırayla dene
        foreach (var dir in shuffledDirections)
        {
            // Sadece 1-2 adım ilerle
            int steps = random.RandiRange(1, 2);
            Vector3 startPos = enemy.GlobalPosition;
            Vector3 targetPos = startPos + dir * steps;

            // Duvar kontrolü için raycast
            var query = PhysicsRayQueryParameters3D.Create(startPos, targetPos);
            query.CollisionMask = 1 << 1; // Layer 1 (duvarlar)
            var result = space.IntersectRay(query);
            
            if (result.Count > 0)
            {
                GD.Print($"[AI] {enemy.Name} hit wall in direction {dir}");
                continue; // Duvar varsa bu yönü atla
            }

            var targetGrid = GridManager.Instance.GetGridObjectFromWorldPosition(targetPos);
            if (targetGrid != null && !targetGrid.IsOccupied && !targetGrid.IsBlocked)
            {
                _patrolTarget = targetPos;
                foundValidTarget = true;
                break;
            }
        }

        if (!foundValidTarget)
        {
            GD.Print($"[AI] {enemy.Name} no valid direction found, ending turn");
            enemy.CompletedTurn = true;
            TurnManager.Instance.EndEnemyMovement();
        }
    }


    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Patrol)
        {
            GD.Print($"[AI] {enemy.Name} changing state from Patrol to {nextState}");
            return nextState;
        }
        // Hedef grid'e hareket et
        if (_patrolTarget != Vector3.Zero && !enemy.CompletedTurn)
        {
            GD.Print($"[AI] {enemy.Name} has patrol target: {_patrolTarget}");
            var targetGrid = GridManager.Instance.GetGridObjectFromWorldPosition(_patrolTarget);
            if (targetGrid != null) 
            {
                GD.Print($"[AI] {enemy.Name} moving to target grid");
                enemy.Move(targetGrid);
                _patrolTarget = Vector3.Zero;
                enemy.CompletedTurn = true;
                TurnManager.Instance.EndEnemyMovement();
            }
        }

        return AIState.Patrol;
    }

    public override void Exit(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Exiting Patrol State");
    }
}
