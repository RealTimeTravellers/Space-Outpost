using Godot;

public class CharacterAimingState : CharacterState
{
    public override void Enter(Character character)
    {
        base.Enter(character);

        
        // EnemyInSight metodunu kullanarak düşman kontrolü
        if (!EnemyInSight(character))
        {
            GD.Print($"[AimingState] {character.Name} no enemies in sight!");
            character.ToggleAim(); // Aim'i kapat
            return;
        }

        // Düşmanları sorgula ve hedef seçimi yap
        character.enemiesInLos = character.QueryForEnemies(
            character.IsFriendly ? TurnManager.Instance.enemyCharacters : TurnManager.Instance.playerCharacters
        );
        
        if (character.Stats.ActionPoints.GetValue() > 0 && character.enemiesInLos.Count > 0)
        {
            character.targetIndex = Mathf.Clamp(character.targetIndex, 0, character.enemiesInLos.Count - 1);
            character.Target = character.enemiesInLos[character.targetIndex];

            character.CharacterController._stateMachine.RequestAnimation("aiming");
            character.LookAt(character.Target.Position);
            CameraManager.Instance.MainCameraSet.LookAt(character.Target.Position);
            CameraManager.MoveToShoulder(character);
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
        // Aim modu kapatıldıysa idle'a dön
        if (!CameraManager.Instance.AimingMode)
            return CharacterStateType.Idle;
            
        return CharacterStateType.Aiming;
    }

    public override void Exit(Character character)
    {
        base.Exit(character);
        CameraManager.ReturnCameraToTactical();
        CameraManager.Instance.AimingMode = false;
    }
}