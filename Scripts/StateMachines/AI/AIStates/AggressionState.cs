using System.Linq;
using Godot;

public class AggressionState : EnemyState
{
    private const float MAX_MOVE_DISTANCE = 10f;

    public async override void Enter(Character enemy)
    {
        GD.Print($"[AI Debug] {enemy.Name} Entering Aggression State");

        enemy.Target = enemy.enemiesInLos[0];
        await enemy.enemyController.HandleAggression();
    }

    public override AIState Process(Character enemy)
    {
        return base.Process(enemy);
    }

    public override void Exit(Character enemy)
    {
        GD.Print($"[AI] {enemy.Name} Exiting Aggression State");
        enemy.Target = null;
    }
}