using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Godot;

public class CharacterReloadingState : CharacterState
{
    private float _reloadTimer = 0f;

    public override void Enter(Character character)
    {
        GD.Print("Entering Reloading State");
        // Reload animasyonunu başlat
    }

    public override CharacterStateType Process(Character character)
    {
        _reloadTimer -= (float)character.GetProcessDeltaTime();
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        // Reload işlemi bitti mi?
        if (_reloadTimer <= 0)
        {
            character.Equipment.CurrentWeapon.Reload();

            // Önceki state'e dön
            if (character.IsInCover)
                return CharacterStateType.TakingCover;
                
            if (Input.IsActionPressed("aim"))
                return CharacterStateType.Aiming;
                
            return CharacterStateType.Idle;
        }

        // Reload işlemi devam ediyor
        return CharacterStateType.Reloading;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Reloading State");
        _reloadTimer = 0f;
    }
}