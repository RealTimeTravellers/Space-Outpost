using Godot;

public class AggressionState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Aggression State");
    }

    public override void Process(Enemy aiController, double delta)
    {
        GD.Print("Attacking the player!");
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Aggression State");
    }
}