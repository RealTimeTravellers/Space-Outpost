using Godot;

public class CharacterIdleState : CharacterState
{
    public override void Enter(Character character)
    {
        base.Enter(character);
        character.CharacterController._stateMachine.RequestAnimation("idle");
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (character.Velocity.Length() > 0.1f && !character.CharacterController._navAgent.IsNavigationFinished())
            return CharacterStateType.Moving;
            
        return CharacterStateType.Idle;
    }
}