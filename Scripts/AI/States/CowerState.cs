using Godot;

public class CowerState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Cower State");
    }

    public override void Process(Enemy aiController, double delta)
    {
        GD.Print("Hiding and taking cover!");
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Cower State");
    }
}