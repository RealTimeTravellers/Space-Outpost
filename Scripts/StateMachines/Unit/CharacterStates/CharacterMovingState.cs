using Godot;
public class CharacterMovingState : CharacterState
{
    public override void Enter(Character character)
    {
        base.Enter(character);
        character.IsMoving = true;
        character.CharacterController._stateMachine.RequestAnimation("moving");
    }

    public override CharacterStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override void Exit(Character character)
    {
        base.Exit(character);
        character.IsMoving = false;
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (character.CharacterController._navAgent.IsNavigationFinished())
            return CharacterStateType.Idle;
        
        return CharacterStateType.Moving;
    }
}

