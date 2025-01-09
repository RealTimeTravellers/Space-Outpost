using Godot;
using System;

public partial class BattleHUD : Control
{
    [Export] public Control GameMenuScene;
    [Export] public LoggingPanelHUD loggingPanel;
    [Export] public CharacterStatPanelHUD characterStatPanel;
    [Export] public CharacterAttackPanelHUD characterAttackPanel;
    private FireType currentFireType = FireType.Attack;
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
        Character character = GridManager.Instance.selectedCharacter;
        if (character != null && character.IsFriendly && character.actionPoints > 0)
            character.ChangeTarget(true);
    }

    private static void OnRigtArrowPressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (character != null && character.IsFriendly && character.actionPoints > 0)
            character.ChangeTarget(false);
    }

    private void OnFirePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (CameraManager.Instance.AimingMode && 
            character != null &&
            character.IsFriendly &&
            character.Target != null &&
            character.actionPoints/* Stats.ActionPoints.GetValue() */ > 0 &&
            character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Aiming)
        {
            if(currentFireType == FireType.Attack && character.gun.currentAmmo > 0)
                character.CharacterController.SetState(CharacterStateType.Shooting, character);
            else if(currentFireType == FireType.SuppressiveFire && character.gun.currentAmmo > 2)
                character.CharacterController.SetState(CharacterStateType.SuppressiveShooting, character);
            characterAttackPanel.OnAimUIUpdate();
            characterStatPanel.UpdateAmmoBox(character);
        }
    }

    public void OnMovePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if(character != null && character.IsFriendly && character.actionPoints > 0)
            character.Move(GridManager.Instance.selectedGrid);
    }

    public void OnReloadPressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if(character != null && character.IsFriendly && character.actionPoints > 0)
            character.Reload();
        characterStatPanel.UpdateAmmoBox(character);
    }

    public void OnStandToEngagePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if(character != null && character.IsFriendly && character.actionPoints > 0)
            character.StandToEngage();
    }

    public void OnSupressiveFirePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (character != null && character.IsFriendly && character.actionPoints > 0)
        {
            character.ToggleAim();
            currentFireType = FireType.SuppressiveFire;
            characterAttackPanel.OnAimUIUpdate();
        }
    }

    public void OnAttackModePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (character != null && character.IsFriendly && character.actionPoints > 0)
        {
            character.ToggleAim();
            currentFireType = FireType.Attack;
            characterAttackPanel.OnAimUIUpdate();
        }
    }
}

