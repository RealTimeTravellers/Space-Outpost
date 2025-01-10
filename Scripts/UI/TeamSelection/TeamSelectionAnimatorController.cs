using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class TeamSelectionAnimatorController : Node
{
    [Export] private float turnAnimationDuration = 0.9f;
    private AnimationTree currentAnimationTree;
    public Action onTurnAnimationComplete;
    public override void _Ready()
    {
        
    }

    public async Task PlayTurnAnimation(Node3D model)
    {      
        currentAnimationTree = model.GetNode<AnimationTree>("AnimationTree");
        if (currentAnimationTree == null || currentAnimationTree.IsQueuedForDeletion()) return;
        
        SetToTurn(currentAnimationTree);
        await Task.Delay(TimeSpan.FromSeconds(turnAnimationDuration));

        if (currentAnimationTree != null && !currentAnimationTree.IsQueuedForDeletion())
        {
            ResetAnimationState(currentAnimationTree);
            onTurnAnimationComplete?.Invoke();
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