using Godot;

public class PlayerIdleState : PlayerState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Idle State");
    }

    public override PlayerStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override PlayerStateType CheckState(Character character)
    {
        if (Input.IsActionPressed("move"))
            return PlayerStateType.Moving;
            
        if (Input.IsActionPressed("aim"))
            return PlayerStateType.Aiming;
            
        if (Input.IsActionPressed("take_cover"))
            return PlayerStateType.TakingCover;
            
        return PlayerStateType.Idle;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Idle State");
    }
}