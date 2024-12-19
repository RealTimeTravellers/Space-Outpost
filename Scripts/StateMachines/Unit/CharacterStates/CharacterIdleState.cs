using Godot;

public class CharacterIdleState : CharacterState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Idle State");
    }

    public override CharacterStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (Input.IsActionPressed("move"))
            return CharacterStateType.Moving;
            
        if (Input.IsActionPressed("aim"))
            return CharacterStateType.Aiming;
            
        if (Input.IsActionPressed("take_cover"))
            return CharacterStateType.InCover;
            
        return CharacterStateType.Idle;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Idle State");
    }
}