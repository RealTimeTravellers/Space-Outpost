using Godot;

public class CharacterState : BaseState<CharacterStateType>
{
    public override void Enter(Character character)
    {
        
    }

    public override void Exit(Character character)
    {

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