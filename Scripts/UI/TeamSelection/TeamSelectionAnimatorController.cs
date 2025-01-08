using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class TeamSelectionAnimatorController : Node
{
    [Export] private float turnAnimationDuration = 3.0f;
    private AnimationTree currentAnimationTree;
    private bool isAnimating = false;
    public Action onTurnAnimationComplete;

    public async void PlayTurnAnimation(Node3D model)
    {
        if (isAnimating) return;
        
        try 
        {
            isAnimating = true;
            currentAnimationTree = model.GetNode<AnimationTree>("AnimationTree");
            if (currentAnimationTree == null || currentAnimationTree.IsQueuedForDeletion()) return;
            
            SetToTurn(currentAnimationTree);
            await Task.Delay(TimeSpan.FromSeconds(turnAnimationDuration));
            
            if (currentAnimationTree == null || currentAnimationTree.IsQueuedForDeletion()) return;
            ResetAnimationState(currentAnimationTree);
            await SetOnTurnAnimationComplete();
        }
        catch (ObjectDisposedException)
        {
            GD.PrintErr("Animation tree was disposed during animation");
        }
        finally
        {
            isAnimating = false;
        }
}

    private void SetToIdle(AnimationTree animTree)
    {
        ResetAnimationState(animTree);
        animTree.Set("parameters/conditions/idle", true);
    }

    private void SetToTurn(AnimationTree animTree)
    {
        ResetAnimationState(animTree);
        animTree.Set("parameters/conditions/turn", true);
    }

    private void ResetAnimationState(AnimationTree animTree)
    {
        if (animTree == null || animTree.IsQueuedForDeletion()) return;
        
        animTree.Set("parameters/conditions/idle", false);
        animTree.Set("parameters/conditions/turn", false);
    }

    private bool IsAnimationPlaying(string animationName)
    {
        return currentAnimationTree.Get($"parameters/conditions/{animationName}").AsBool();
    }

    public async Task SetOnTurnAnimationComplete()
    {
        while (IsAnimationPlaying("turn"))
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
        }
        onTurnAnimationComplete?.Invoke();
    }


    
}