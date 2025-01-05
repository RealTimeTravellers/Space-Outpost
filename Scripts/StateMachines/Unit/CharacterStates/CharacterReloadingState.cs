using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


public class CharacterReloadingState : CharacterState
{
    private bool isReloadingAnimationStarted = false;
    private bool isReloadingAnimationFinished = false;
    private float reloadingAnimationTimer = 0f;
    private const float RELOADING_ANIMATION_DURATION = 1.9f; 

    public override void Enter(Character character)
    {
        base.Enter(character);
        character.CompleteAction(2);
        isReloadingAnimationStarted = false;
        isReloadingAnimationFinished = false;
        reloadingAnimationTimer = 0f;
        character.CharacterController._stateMachine.RequestAnimation("reloading");
        isReloadingAnimationStarted = true;
        character.gun.Reload();
    }

    public override CharacterStateType Process(Character character)
    {
        if (isReloadingAnimationStarted && !isReloadingAnimationFinished)
        {
            reloadingAnimationTimer += (float)character.GetProcessDeltaTime();
            
            if (reloadingAnimationTimer >= RELOADING_ANIMATION_DURATION)
            {
                isReloadingAnimationFinished = true;
            }
        }
        
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (isReloadingAnimationStarted && isReloadingAnimationFinished)
        {
            return character.CharacterController._stateMachine.PreviousStateType;
        }
        
        return CharacterStateType.Reloading;
    }

    public override void Exit(Character character)
    {
        character.AnimatorController.ResetAllAnimationStates();
    }
}
