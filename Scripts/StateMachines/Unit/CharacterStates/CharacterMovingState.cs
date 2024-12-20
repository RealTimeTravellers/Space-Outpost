using Godot;

public class CharacterMovingState : CharacterState
{
    public override void Enter(Character character)
    {
        base.Enter(character);
        character.CharacterController.GetStateMachine().RequestAnimation("move");
    }

    public override CharacterStateType CheckState(Character character)
    {
        // NavAgent'in durumunu kontrol et
        if (character.CharacterController._navAgent.IsNavigationFinished())
        {
            return CharacterStateType.Idle;
        }
        
        return CharacterStateType.Moving;
    }

    public override void Exit(Character character)
    {
        base.Exit(character);
        character.CharacterController.GetStateMachine().RequestAnimation("idle");
    }
}