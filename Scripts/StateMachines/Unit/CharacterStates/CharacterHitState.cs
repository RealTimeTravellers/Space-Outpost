using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class CharacterHitState : CharacterState
{
    private bool isHitAnimationStarted = false;
    private bool isHitAnimationFinished = false;
    private float hitAnimationTimer = 0f;
    private const float HIT_ANIMATION_DURATION = 0.9f; 

    public override void Enter(Character character)
    {
        base.Enter(character);
        isHitAnimationStarted = false;
        isHitAnimationFinished = false;
        hitAnimationTimer = 0f;
        character.CharacterController._stateMachine.RequestAnimation("hit");
        isHitAnimationStarted = true;
    }

    public override CharacterStateType Process(Character character)
    {
        if (isHitAnimationStarted && !isHitAnimationFinished)
        {
            hitAnimationTimer += (float)character.GetProcessDeltaTime();
            
            if (hitAnimationTimer >= HIT_ANIMATION_DURATION)
            {
                isHitAnimationFinished = true;
            }
        }
        
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (isHitAnimationStarted && isHitAnimationFinished)
        {
            return character.CharacterController._stateMachine.PreviousStateType;
        }
        
        return CharacterStateType.Hit;
    }

    public override void Exit(Character character)
    {
        character.AnimatorController.ResetAllAnimationStates();
    }
}