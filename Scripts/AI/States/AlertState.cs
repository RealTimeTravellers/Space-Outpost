using Godot;

public class AlertState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Alert State");
    }

    public override AIState Process(Enemy enemy)
    {
        GD.Print("Alert! Searching for player...");
        return AIState.Alert;
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Alert State");
    }
}