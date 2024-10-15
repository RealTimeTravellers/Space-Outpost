using Godot;

public class PatrolState : BaseState
{
    public override void Enter(Enemy enemy)
    {
        GD.Print("Entering Patrol State");
    }

    public override AIState Process(Enemy enemy)
    {
        GD.Print("Patrolling!");
        return AIState.Patrol;
    }

    public override void Exit(Enemy enemy)
    {
        GD.Print("Exiting Patrol State");
    }
}
