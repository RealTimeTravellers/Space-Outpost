using Godot;
using System.Linq;

public class CharacterAimingState : CharacterState
{
    public override void Enter(Character character)
    {
        base.Enter(character);

        if (!EnemyInSight(character))
        {
            character.ToggleAim(); 
            return;
        }
        
        character.enemiesInLos = character.QueryForEnemies(new Godot.Collections.Array<Character>(
            character.IsFriendly ? 
            TurnManager.Instance.enemyCharacters.Where(e => e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death).ToList() : 
            TurnManager.Instance.playerCharacters.Where(e => e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death).ToList()
        ));
        
        if (character.actionPoints/* Stats.ActionPoints.GetValue() */ > 0 && character.enemiesInLos.Count > 0)
        {
            character.targetIndex = Mathf.Clamp(character.targetIndex, 0, character.enemiesInLos.Count - 1);
            character.Target = character.enemiesInLos[character.targetIndex];

            character.CharacterController._stateMachine.RequestAnimation("aiming");
            character.LookAt(character.Target.Position);

            if (character.IsFriendly)
            {
                CameraManager.Instance.MainCameraSet.LookAt(character.Target.Position);
                CameraManager.MoveToShoulder(character);
            }
            
            character.RotateY(Mathf.Pi);
        }
        else
        {
            CameraManager.Instance.AimingMode = false;
        }
    }

    public override CharacterStateType Process(Character character)
    {
        // Sol/Sağ ok tuşlarıyla hedef değiştirme
        if (Input.IsActionJustPressed("ui_left"))
            character.ChangeTarget(true);
        else if (Input.IsActionJustPressed("ui_right"))
            character.ChangeTarget(false);
            
        return CheckState(character);
    }

    public override CharacterStateType CheckState(Character character)
    {
        if (character.IsFriendly && character.CharacterController._stateMachine.PreviousStateType == CharacterStateType.Shooting)
        {
            if (!CameraManager.Instance.AimingMode)
                return CharacterStateType.Idle;
        }
        else if (!character.IsFriendly && character.CharacterController._stateMachine.PreviousStateType == CharacterStateType.Shooting)
        {
            return CharacterStateType.Idle;
        }
        
        return CharacterStateType.Aiming;
    }

    public override void Exit(Character character)
    {
        base.Exit(character);
        CameraManager.Instance.AimingMode = false;
    }
}