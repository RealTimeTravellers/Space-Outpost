using Godot;

public class CharacterInCoverState : CharacterState
{
    private bool coverExiting;
    private bool outCoverAnimationStarted;

    public override void Enter(Character character)
    {
        base.Enter(character);
        coverExiting = false;
        outCoverAnimationStarted = false;
        character.IsInCover = true;
        character.Stats.Evasion.AddModifier(15);
        character.CharacterController._stateMachine.RequestAnimation("incover");
    }

    public override CharacterStateType Process(Character character)
    {
        if(character.IsMoving && !outCoverAnimationStarted)
        {
            outCoverAnimationStarted = true;
            coverExiting = true;
            character.CharacterController._stateMachine.RequestAnimation("outcover");
            return CharacterStateType.InCover;
        }

        if(coverExiting && outCoverAnimationStarted)
        {
            // Animasyon bittiğinde Idle'a geç
            if(!character.AnimatorController.IsAnimationPlaying("outcover"))
            {
                return CharacterStateType.Idle;
            }
        }

        return CharacterStateType.InCover;
    }

    public override void Exit(Character character)
    {
        coverExiting = false;
        outCoverAnimationStarted = false;
        character.IsInCover = false;
        character.Stats.Evasion.RemoveModifier(15);
    }
}