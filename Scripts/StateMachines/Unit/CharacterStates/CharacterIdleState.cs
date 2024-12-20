using Godot;

public class CharacterIdleState : CharacterState
{
    public override void Enter(Character character)
    {
        base.Enter(character);
        character.CharacterController.GetStateMachine().RequestAnimation("idle");
    }

    public override CharacterStateType CheckState(Character character)
    {
        return CharacterStateType.Idle;
    }
}