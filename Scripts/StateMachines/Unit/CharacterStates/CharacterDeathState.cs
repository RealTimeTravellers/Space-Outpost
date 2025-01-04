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
            
            GD.Print("CharacterDeathState: Enter");
            character.CharacterController._stateMachine.RequestAnimation("death");
            
            // Delay the Die() call until animation starts
            character.Die();
        }
    }

    public override CharacterStateType Process(Character character)
    {
        if (_deathProcessed)
            return CharacterStateType.Death;
            
        //return base.Process(character);
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

