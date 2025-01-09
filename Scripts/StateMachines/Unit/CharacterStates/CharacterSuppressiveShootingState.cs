using Godot;

public class CharacterSuppressiveShootingState : CharacterState
{
    private bool _hasShot = false;

    public override async void Enter(Character character)
    {
        _hasShot = false;  

        if (character.actionPoints/* Stats.ActionPoints.GetValue() */ <= 0)
        {
            return;
        }

        // If target is not null, shoot
        if (character.Target != null)
        {
            character.CharacterController._stateMachine.RequestAnimation("shooting");
            // Shot fired
            if (character.IsFriendly)
            {
                var targetGrid = GridManager.Instance.GetGridObjectFromWorldPosition(character.Target.GlobalPosition);
                EnemyManager.Instance.ReportShotFired(targetGrid);
            }

            await character.SuppressiveFire(character.Target);
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
        // If shooting animation is finished, go to aiming state
        if (_hasShot && !character.AnimatorController.IsAnimationPlaying("shooting"))
        {
            //CameraManager.ReturnCameraToTactical();
            return CharacterStateType.Aiming; 
        }
        return CharacterStateType.SuppressiveShooting;
    }
    public override void Exit(Character character)
   {
       _hasShot = false;
   }
}
