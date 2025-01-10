using Godot;

public class CharacterIdleState : CharacterState
{
    public override void Enter(Character character)
    {
        base.Enter(character);
        character.IsInCover = false;
        character.CharacterController._stateMachine.RequestAnimation("idle");
    }

    public override CharacterStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (character.Velocity.Length() > 0.1f && !character.CharacterController._navAgent.IsNavigationFinished())
            return CharacterStateType.Moving;
        
        if(character.CharacterController._stateMachine.PreviousStateType != CharacterStateType.InCover){
            var nearestCover = character.QueryForCover();
            if (nearestCover != null && 
                character.GlobalPosition.DistanceTo(nearestCover.GlobalPosition) <= 2f)
                return CharacterStateType.InCover;
        }

        if (character.Health <= 0 || character.Health/* Stats.Health.GetValue() */ <= 0)
            return CharacterStateType.Death;
        
        return CharacterStateType.Idle;
    }
}