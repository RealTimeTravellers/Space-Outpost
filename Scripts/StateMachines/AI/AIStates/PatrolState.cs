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
        GD.Print($"[AI] {enemy.Name} choosing direction with AP: {currentAP}");

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
            GD.Print($"[AI] {enemy.Name} trying direction {dir}, steps: {steps}");

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
                GD.Print($"[AI] {enemy.Name} found valid target at: {_patrolTarget}");
                break;
            }
        }

        if (!foundValidTarget)
        {
            GD.Print($"[AI] {enemy.Name} no valid direction found, ending turn");
            enemy.Stats.DepleteActionPoints();
            enemy.CompletedTurn = true;
            TurnManager.Instance.EndEnemyMovement();
        }
    }


    public override AIState Process(Character enemy)
    {
        var nextState = base.CheckState(enemy);
        if (nextState != AIState.Patrol)
            return nextState;

        var currentAP = enemy.Stats.ActionPoints.GetValue();
        GD.Print($"[AI] {enemy.Name} Processing with AP: {currentAP}");
        
        if (currentAP <= 0)
        {
            GD.Print($"[AI] {enemy.Name} has no AP left, completing turn");
            enemy.CompletedTurn = true;
            TurnManager.Instance.EndEnemyMovement();
            return AIState.Patrol;
        }

        // Hedef grid'e hareket et
        if (_patrolTarget != Vector3.Zero)
        {
            var targetGrid = GridManager.Instance.GetGridObjectFromWorldPosition(_patrolTarget);
            if (targetGrid != null)
            {
                enemy.Move(targetGrid);
            }
        }
        else
        {
            ChooseNewDirection(enemy);
        }

        return AIState.Patrol;
    }

    public override void Exit(Character enemy)
    {
        //GD.Print("Exiting Patrol State");
    }
}
