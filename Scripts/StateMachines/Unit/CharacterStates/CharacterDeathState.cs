using System;
using Godot;

public class CharacterDeathState : CharacterState
{
    private bool _deathAnimationStarted = false;
    private bool _deathProcessed = false;

    public override void Enter(Character character)
    {
        if (!_deathAnimationStarted)
        {
            _deathAnimationStarted = true;

            if (!character.IsFriendly)
            {
                var aiController = character.GetNode<EnemyAIController>("EnemyAIController");
                if (aiController != null)
                {
                    //aiController.PrepareForDestruction();
                }
            }

            character.CharacterController._stateMachine.RequestAnimation("death");
            character.Die();
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

