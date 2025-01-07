using Godot;

public class CharacterInCoverState : CharacterState
{
    private bool coverExiting;
    private bool outCoverAnimationStarted;
    private float outCoverTimer = 0;
    private const float OUT_COVER_ANIMATION_DURATION = 0.8f;

    public override void Enter(Character character)
    {
        MissionManager.Instance.RecordCoverUse();
        base.Enter(character);
        coverExiting = false;
        outCoverAnimationStarted = false;
        outCoverTimer = 0;
        character.IsInCover = true;
        character.Evasion += 15;
        character.CharacterController._stateMachine.RequestAnimation("incover");
    }

    public override CharacterStateType Process(Character character)
    {
        if (character.IsMoving && !outCoverAnimationStarted)
        {
            GD.Print("[Debug] Starting outcover animation");
            outCoverAnimationStarted = true;
            coverExiting = true;
            outCoverTimer = 0;
            character.CharacterController._stateMachine.RequestAnimation("outcover");
            return CharacterStateType.InCover;
        }

        if (coverExiting && outCoverTimer >= OUT_COVER_ANIMATION_DURATION)
        {
            GD.Print("[Debug] Outcover animation completed, switching to Idle");
            character.IsInCover = false;
            character.Evasion -= 15;
            character.CharacterController._stateMachine.RequestAnimation("idle");
            return CharacterStateType.Idle;
        }

        if (coverExiting)
        {
            outCoverTimer += 1/(float)Engine.GetFramesPerSecond();
        }

        return CharacterStateType.InCover;
    }

    public override void Exit(Character character)
    {
        base.Exit(character);
        // Exit'te flag'leri sıfırla
        character.IsInCover = false;
        character.Evasion -= 15;
        coverExiting = false;
        outCoverAnimationStarted = false;
    }
}