using Godot;

public class CharacterInCoverState : CharacterState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Leaving Cover State");
    }

    public override CharacterStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        // Eğer cover'dan çıkma animasyonu bittiyse
        // karakterin cover'da olup olmadığını kontrol et
        if (!character.IsInCover)
        {
            if (Input.IsActionPressed("aim"))
                return CharacterStateType.Aiming;
                
            if (Input.IsActionPressed("move"))
                return CharacterStateType.Moving;
                
            return CharacterStateType.Idle;
        }
        
        // Cover'dan çıkma işlemi devam ediyor
        return CharacterStateType.InCover;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Leaving Cover State");
    }
}