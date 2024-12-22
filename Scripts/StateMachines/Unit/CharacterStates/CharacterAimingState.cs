using Godot;

public class CharacterAimingState : CharacterState
{
    public override void Enter(Character character)
    {
        base.Enter(character);
        character.CharacterController._stateMachine.RequestAnimation("aiming");
        
        // EnemyInSight metodunu kullanarak düşman kontrolü
        if (!EnemyInSight(character))
        {
            GD.Print($"[AimingState] {character.Name} no enemies in sight!");
            CameraManager.ReturnCameraToTactical();
            character.CharacterController.SetState(CharacterStateType.Idle, character);
            return;
        }

        // Düşmanları sorgula
        var enemies = character.IsFriendly ? 
            TurnManager.Instance.enemyCharacters : 
            TurnManager.Instance.playerCharacters;
            
        character.enemiesInLos = character.QueryForEnemies(enemies);
        GD.Print($"[AimingState] {character.Name} found {character.enemiesInLos.Count} enemies in LOS");
        
        // Kamera ayarları ve hedef seçimi
        if (character.Stats.ActionPoints.GetValue() > 0 && character.enemiesInLos.Count > 0)
        {
            character.targetIndex = Mathf.Clamp(character.targetIndex, 0, character.enemiesInLos.Count - 1);
            character.Target = character.enemiesInLos[character.targetIndex];

            character.LookAt(character.Target.Position);
            CameraManager.Instance.MainCameraSet.LookAt(character.Target.Position);
            CameraManager.MoveToShoulder(character);
            character.RotateY(Mathf.Pi);
        }
        else
        {
            CameraManager.ReturnCameraToTactical();
            character.CharacterController.SetState(CharacterStateType.Idle, character);
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
    }
}