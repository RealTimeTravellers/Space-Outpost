using Godot;
public class CharacterMovingState : CharacterState
{
    public override void Enter(Character character)
    {
        base.Enter(character);
        character.CharacterController._stateMachine.RequestAnimation("moving");
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (character.CharacterController._navAgent.IsNavigationFinished())
            return CharacterStateType.Idle;
            
        return CharacterStateType.Moving;
    }
}

