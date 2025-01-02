using Godot;
using System;

public partial class BattleHUD : Control
{
    [Export] public Label DamageLabel;
    [Export] public Label AccuracyLabel;
    [Export] public Label CriticalHitChanceLabel;
    [Export] public Label EvasionLabel;
    [Export] public Button fireButton;
    [Export] public Control GameMenuScene;
    
    private Character ChangeSelectedCharacter(bool toLeft)
    {
        return null;
    }

    private void OnMouseEnteredUI()
    {
        Raycaster.MouseOverUI = true;
    }

    private void OnMouseExitedUI()
    {
        Raycaster.MouseOverUI = false;
    }

    private static void OnLeftArrowPressed()
    {
        if (CameraManager.Instance.AimingMode)
            GridManager.Instance.selectedCharacter.ChangeTarget(true);
        else
        {
            // TODO: add character swithing
        }
    }

    private static void OnRigtArrowPressed()
    {
        if (CameraManager.Instance.AimingMode)
            GridManager.Instance.selectedCharacter.ChangeTarget(false);
        else
        {
            // TODO: add character swithing
        }
    }

    private static void OnFirePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (CameraManager.Instance.AimingMode && 
            character.Target != null &&
            character.actionPoints/* Stats.ActionPoints.GetValue() */ > 0 &&
            character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Aiming)
        {
            character.CharacterController.SetState(CharacterStateType.Shooting, character);
        }
    }

    private static void OnMovePressed()
    {
        if(GridManager.Instance.selectedCharacter.IsFriendly)
            GridManager.Instance.selectedCharacter.Move(GridManager.Instance.selectedGrid);
    }

    private static void OnStandToEngagePressed()
    {
        GridManager.Instance.selectedCharacter.StandToEngage();
    }

    private static void OnSupressiveFirePressed()
    {
        CameraManager.AreaSelectionMode();
        GridManager.Instance.selectedCharacter.SupressiveFire();
    }

    private void OnAttackModePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (character.actionPoints/* Stats.ActionPoints.GetValue() */ <= 0)
            return;

        character.ToggleAim();
        OnAimUIUpdate();

    }

    private void OnAimUIUpdate()
    {
        if (CameraManager.Instance.AimingMode)
        {
            fireButton.Visible = true;
            UpdateAttackPanel();
        }
        else
            fireButton.Visible = false;
    }

    private void UpdateAttackPanel()
    {
        var character = GridManager.Instance.selectedCharacter;
        var target = character.Target;
        var armorValue = target.Stats.Armor.GetValue();
        
        // Base damage
        int baseDamage = 7; 
        
        int minDamage = Mathf.Max(1, baseDamage - armorValue);
        int maxDamage = baseDamage;
        
        DamageLabel.Text = $"Damage: {minDamage} - {maxDamage}";
        AccuracyLabel.Text = "Accuracy: " + (character.Stats.Accuracy.GetValue() - target.Stats.Evasion.GetValue()).ToString();
        CriticalHitChanceLabel.Text = "Crit Chance: " + character.Stats.CriticalHitChance.GetValue().ToString();
        EvasionLabel.Text = "Evasion: " + target.Stats.Evasion.GetValue().ToString();
    }
}
