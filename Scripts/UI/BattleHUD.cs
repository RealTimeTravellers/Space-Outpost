using Godot;
using System;

public partial class BattleHUD : Control
{
    [Export] public Control GameMenuScene;
    [Export] public LoggingPanelHUD loggingPanel;
    [Export] public CharacterStatPanelHUD characterStatPanel;
    [Export] public CharacterAttackPanelHUD characterAttackPanel;
    public override void _Ready()
    {
        MissionManager.Instance.InitializeLogger(loggingPanel);
    }
    
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

    private void OnFirePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (CameraManager.Instance.AimingMode && 
            character.Target != null &&
            character.actionPoints/* Stats.ActionPoints.GetValue() */ > 0 &&
            character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Aiming)
        {
            character.CharacterController.SetState(CharacterStateType.Shooting, character);
            characterAttackPanel.OnAimUIUpdate();
            characterStatPanel.UpdateAmmoBox(character);
        }
    }

    public void OnMovePressed()
    {
        if(GridManager.Instance.selectedCharacter.IsFriendly)
            GridManager.Instance.selectedCharacter.Move(GridManager.Instance.selectedGrid);
    }

    public void OnReloadPressed()
    {
        GridManager.Instance.selectedCharacter.Reload();
        characterStatPanel.UpdateAmmoBox(GridManager.Instance.selectedCharacter);
    }

    public void OnStandToEngagePressed()
    {
        GridManager.Instance.selectedCharacter.StandToEngage();
    }

    public void OnSupressiveFirePressed()
    {
        CameraManager.AreaSelectionMode();
        GridManager.Instance.selectedCharacter.SupressiveFire();
    }

    public void OnAttackModePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (character.actionPoints/* Stats.ActionPoints.GetValue() */ <= 0)
            return;

        character.ToggleAim();
        characterAttackPanel.OnAimUIUpdate();
    }
}
