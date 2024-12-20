using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class CharacterAimingState : CharacterState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Aiming State");
    }

    public override CharacterStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (Input.IsActionPressed("shoot"))
            return CharacterStateType.Shooting;
            
        if (!Input.IsActionPressed("aim"))
            return CharacterStateType.Idle;
            

            
        return CharacterStateType.Aiming;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Aiming State");
    }
}