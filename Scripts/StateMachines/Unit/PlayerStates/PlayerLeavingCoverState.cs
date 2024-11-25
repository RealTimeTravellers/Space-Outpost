using Godot;

public class PlayerLeavingCoverState : PlayerState
{
    public override void Enter(Character character)
    {
        GD.Print("Entering Leaving Cover State");
    }

    public override PlayerStateType Process(Character character)
    {
        return CheckState(character);
    }

    public override PlayerStateType CheckState(Character character)
    {
        // Eğer cover'dan çıkma animasyonu bittiyse
        // karakterin cover'da olup olmadığını kontrol et
        if (!character.IsInCover())
        {
            if (Input.IsActionPressed("aim"))
                return PlayerStateType.Aiming;
                
            if (Input.IsActionPressed("move"))
                return PlayerStateType.Moving;
                
            return PlayerStateType.Idle;
        }
        
        // Cover'dan çıkma işlemi devam ediyor
        return PlayerStateType.LeavingCover;
    }

    public override void Exit(Character character)
    {
        GD.Print("Exiting Leaving Cover State");
    }
}