using Godot;

public class TacticalState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Tactical State");
    }

    public override void Process(Enemy aiController, double delta)
    {
        GD.Print("Taking strategic positions!");
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Tactical State");
    }
}