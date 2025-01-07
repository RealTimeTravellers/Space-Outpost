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

        character.TakeCover(enterCover: true);
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

        if (coverExiting)
        {
            outCoverTimer += 1/(float)Engine.GetFramesPerSecond();
            if (outCoverTimer >= OUT_COVER_ANIMATION_DURATION)
                return CharacterStateType.Idle;
        }

        return CharacterStateType.InCover;
    }

    public override void Exit(Character character)
    {
        base.Exit(character);
        character.TakeCover(enterCover: false);
    }
}