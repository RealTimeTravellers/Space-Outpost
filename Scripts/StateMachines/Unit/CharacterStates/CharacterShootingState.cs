using Godot;

public class CharacterShootingState : CharacterState
{
    private bool _hasShot = false;

    public override void Enter(Character character)
    {
        GD.Print("[ShootingState] Entering Shooting State");
        _hasShot = false;  

        if (character.Stats.ActionPoints.GetValue() <= 0)
        {
            GD.Print("[ShootingState] No action points, cannot shoot");
            return;
        }

        // Hedef varsa ateş et
        if (character.Target != null)
        {
            GD.Print("[ShootingState] Requesting shooting animation");
            character.CharacterController._stateMachine.RequestAnimation("shooting");
            character.Attack(character.Target);
            _hasShot = true;
        }
        else
        {
            GD.Print("[ShootingState] No target to shoot at");
        }
    }
    public override CharacterStateType Process(Character character)
   {
       return CheckState(character);
   }
    public override CharacterStateType CheckState(Character character)
    {
        // Ateş etme animasyonu bittiyse
        if (_hasShot && !character.AnimatorController.IsAnimationPlaying("shooting"))
        {
            GD.Print("[ShootingState] Shooting animation completed");
            return CharacterStateType.Aiming; // Önce aiming'e dön
        }
        return CharacterStateType.Shooting;
    }
    public override void Exit(Character character)
   {
       GD.Print("Exiting Shooting State");
       _hasShot = false;
   }
}
