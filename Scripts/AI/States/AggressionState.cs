using Godot;

public class AggressionState : BaseState
{
    public override void Enter(Enemy aiController)
    {
        GD.Print("Entering Aggression State");
    }

    public override AIState Process(Enemy enemy)
    {
        GD.Print("Finding the player!");
        return AIState.Aggression;
    }

    public override void Exit(Enemy aiController)
    {
        GD.Print("Exiting Aggression State");
    }
}