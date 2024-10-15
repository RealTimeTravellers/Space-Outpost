using Godot;

public class TacticalState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Tactical State");
    }

    public override AIState Process(Enemy enemy)
    {
        GD.Print("Tactical Positioning!");
        return AIState.Tactical;
    }
    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Tactical State");
    }
}