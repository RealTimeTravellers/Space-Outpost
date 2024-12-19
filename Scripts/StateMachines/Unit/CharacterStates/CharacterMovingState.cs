using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class CharacterMovingState : CharacterState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Moving State");
    }

    public override CharacterStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (!Input.IsActionPressed("move"))
            return CharacterStateType.Idle;
            
        if (Input.IsActionPressed("aim"))
            return CharacterStateType.Aiming;
            
        if (Input.IsActionPressed("take_cover"))
            return CharacterStateType.InCover;
            
        return CharacterStateType.Moving;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Moving State");
    }
}