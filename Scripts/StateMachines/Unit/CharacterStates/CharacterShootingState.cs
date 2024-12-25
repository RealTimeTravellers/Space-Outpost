using Godot;

public class CharacterShootingState : CharacterState
{
    private bool _hasShot = false;

    public override void Enter(Character character)
    {
        _hasShot = false;  

        if (character.Stats.ActionPoints.GetValue() <= 0)
        {
            return;
        }

        // Hedef varsa ateş et
        if (character.Target != null)
        {
            character.CharacterController._stateMachine.RequestAnimation("shooting");
            character.Attack(character.Target);
            _hasShot = true;
        }
        else
        {
            return;
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
            return CharacterStateType.Aiming; // Önce aiming'e dön
        }
        return CharacterStateType.Shooting;
    }
    public override void Exit(Character character)
   {
       _hasShot = false;
   }
}
