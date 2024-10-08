using Godot;

public class PatrolState : IBaseState
{
    public void Enter(Enemy enemy)
    {
        GD.Print("Entering Patrol State");
    }

    public void Process(Enemy enemy, double delta)
    {
        GD.Print("Patrolling...");
    }

    public void Exit(Enemy enemy)
    {
        GD.Print("Exiting Patrol State");
    }
}
