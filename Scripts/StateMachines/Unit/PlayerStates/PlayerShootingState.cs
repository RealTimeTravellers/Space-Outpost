using Godot;

public class PlayerShootingState : PlayerState
{
    private float _shootCooldown = 0f;

    public override void Enter(Character character)
    {
        GD.Print("Entering Shooting State");
        _shootCooldown = character.Equipment.CurrentWeapon.FireRate;
        
        // Silahı ateşle
        character.Equipment.CurrentWeapon.Fire();
    }

    public override PlayerStateType Process(Character character)
    {
        _shootCooldown -= (float)character.GetProcessDeltaTime();
        return CheckState(character);
    }

    public override PlayerStateType CheckState(Character character)
    {
        // Mermi bitti mi?
        if (character.Equipment.CurrentWeapon.NeedsReload())
            return PlayerStateType.Reloading;

        // Ateş etme cooldown'ı bitti mi?
        if (_shootCooldown <= 0)
        {
            // Hala ateş tuşuna basılı mı?
            if (Input.IsActionPressed("shoot") && Input.IsActionPressed("aim"))
            {
                // Tekrar ateş et
                character.Equipment.CurrentWeapon.Fire();
                _shootCooldown = character.Equipment.CurrentWeapon.FireRate;
                return PlayerStateType.Shooting;
            }
            
            // Ateş tuşu bırakıldı
            if (Input.IsActionPressed("aim"))
                return PlayerStateType.Aiming;
                
            return PlayerStateType.Idle;
        }

        // Ateş etme cooldown'ı devam ediyor
        return PlayerStateType.Shooting;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Shooting State");
        _shootCooldown = 0f;
    }
}