using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class CharacterTakingCoverState : CharacterState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Taking Cover State");
    }

    public override CharacterStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (Input.IsActionPressed("leave_cover"))
            return CharacterStateType.LeavingCover;
            
        if (Input.IsActionPressed("aim"))
            return CharacterStateType.Aiming;
            
        if (Input.IsActionPressed("reload"))
            return CharacterStateType.Reloading;
            
        return CharacterStateType.TakingCover;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Taking Cover State");
    }
}