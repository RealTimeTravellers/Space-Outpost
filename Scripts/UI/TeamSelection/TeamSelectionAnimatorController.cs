using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class TeamSelectionAnimatorController : Node
{
    [Export] private float turnAnimationDuration = 1.0f;

    public async void PlayTurnAnimation(Node3D model)
    {
        var animTree = model.GetNode<AnimationTree>("AnimationTree");
        SetToTurn(animTree);
        await Task.Delay(TimeSpan.FromSeconds(turnAnimationDuration));
        ResetAnimationState(animTree);
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
        animTree.Set("parameters/conditions/idle", false);
        animTree.Set("parameters/conditions/turn", false);
    }
}