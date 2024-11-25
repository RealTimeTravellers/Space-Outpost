using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class PlayerTakingCoverState : PlayerState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Taking Cover State");
    }

    public override PlayerStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override PlayerStateType CheckState(Character character)
    {
        if (Input.IsActionPressed("leave_cover"))
            return PlayerStateType.LeavingCover;
            
        if (Input.IsActionPressed("aim"))
            return PlayerStateType.Aiming;
            
        if (Input.IsActionPressed("reload"))
            return PlayerStateType.Reloading;
            
        return PlayerStateType.TakingCover;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Taking Cover State");
    }
}