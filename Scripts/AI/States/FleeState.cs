using Godot;

public class FleeState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Flee State");
    }

    public override void Process(Enemy aiController, double delta)
    {
        GD.Print("Running away from danger!");
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Flee State");
    }
}