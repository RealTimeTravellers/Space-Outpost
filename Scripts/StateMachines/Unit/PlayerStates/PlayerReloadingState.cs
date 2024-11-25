using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Godot;

public class PlayerReloadingState : PlayerState
{
    private float _reloadTimer = 0f;

    public override void Enter(Character character)
    {
        GD.Print("Entering Reloading State");
        _reloadTimer = character.Equipment.CurrentWeapon.ReloadTime;
        // Reload animasyonunu başlat
    }

    public override PlayerStateType Process(Character character)
    {
        _reloadTimer -= (float)character.GetProcessDeltaTime();
        return CheckState(character);
    }

    public override PlayerStateType CheckState(Character character)
    {
        // Reload işlemi bitti mi?
        if (_reloadTimer <= 0)
        {
            character.Equipment.CurrentWeapon.Reload();
            
            // Önceki state'e dön
            if (character.IsInCover())
                return PlayerStateType.TakingCover;
                
            if (Input.IsActionPressed("aim"))
                return PlayerStateType.Aiming;
                
            return PlayerStateType.Idle;
        }

        // Reload işlemi devam ediyor
        return PlayerStateType.Reloading;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Reloading State");
        _reloadTimer = 0f;
    }
}