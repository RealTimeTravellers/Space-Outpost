using System;
using Godot;

public class CharacterDeathState : CharacterState
{
    private bool _deathAnimationStarted = false;
    private bool _deathProcessed = false;

    public override void Enter(Character character)
    {
        GD.Print("[DeathState] Enter called");
        if (!_deathAnimationStarted)
        {
            _deathAnimationStarted = true;
            GD.Print("[DeathState] Starting death animation");

            if (!character.IsFriendly)
            {
                var aiController = character.GetNode<EnemyAIController>("EnemyAIController");
                if (aiController != null)
                {
                    GD.Print("[DeathState] Disabling AI Controller");
                    //aiController.PrepareForDestruction();
                }
            }

            character.CharacterController._stateMachine.RequestAnimation("death");
            character.Die();
            GD.Print("[DeathState] Death animation requested");
        }
    }

    public override CharacterStateType Process(Character character)
    {
        base.Process(character);
        return CharacterStateType.Death;
    }

    public override CharacterStateType CheckState(Character character)
    {
        return CharacterStateType.Death;
    }

    public override void Exit(Character character)
    {
        // Hiçbir zaman çıkış yapılmayacak
    }
}

