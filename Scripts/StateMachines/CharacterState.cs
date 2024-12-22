using Godot;

public class CharacterState : BaseState<CharacterStateType>
{
    public override void Enter(Character character)
    {
        GD.Print($"[Character State] Entering {GetType().Name}");
    }

    public override void Exit(Character character)
    {
        GD.Print($"[Character State] Exiting {GetType().Name}");
    }

    public override CharacterStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        return CharacterStateType.Idle;
    }
}