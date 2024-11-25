using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class PlayerMovingState : PlayerState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Moving State");
    }

    public override PlayerStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override PlayerStateType CheckState(Character character)
    {
        if (!Input.IsActionPressed("move"))
            return PlayerStateType.Idle;
            
        if (Input.IsActionPressed("aim"))
            return PlayerStateType.Aiming;
            
        if (Input.IsActionPressed("take_cover"))
            return PlayerStateType.TakingCover;
            
        return PlayerStateType.Moving;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Moving State");
    }
}