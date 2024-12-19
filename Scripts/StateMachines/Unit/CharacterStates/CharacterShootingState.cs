using Godot;

public class CharacterShootingState : CharacterState
{
    private float _shootCooldown = 0f;

    public override void Enter(Character character)
    {
        GD.Print("Entering Shooting State");
        // Silahı ateşle
        character.Equipment.CurrentWeapon.Fire();
    }

    public override CharacterStateType Process(Character character)
    {
        _shootCooldown -= (float)character.GetProcessDeltaTime();
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {

        // Ateş etme cooldown'ı bitti mi?
        if (_shootCooldown <= 0)
        {
            // Hala ateş tuşuna basılı mı?
            if (Input.IsActionPressed("shoot") && Input.IsActionPressed("aim"))
            {
                // Tekrar ateş et
                character.Equipment.CurrentWeapon.Fire();
                return CharacterStateType.Shooting;
            }
            
            // Ateş tuşu bırakıldı
            if (Input.IsActionPressed("aim"))
                return CharacterStateType.Aiming;
                
            return CharacterStateType.Idle;
        }

        // Ateş etme cooldown'ı devam ediyor
        return CharacterStateType.Shooting;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Shooting State");
        _shootCooldown = 0f;
    }
}