using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class PlayerAimingState : PlayerState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Aiming State");
    }

    public override PlayerStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override PlayerStateType CheckState(Character character)
    {
        if (Input.IsActionPressed("shoot"))
            return PlayerStateType.Shooting;
            
        if (!Input.IsActionPressed("aim"))
            return PlayerStateType.Idle;
            
        if (Input.IsActionPressed("reload"))
            return PlayerStateType.Reloading;
            
        return PlayerStateType.Aiming;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Aiming State");
    }
}