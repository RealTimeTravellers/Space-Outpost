using Godot;
using System;

public partial class BattleHUD : Control
{
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
            character.Stats.ActionPoints.GetValue() > 0 &&
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

    private static void OnAttackModePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (character.Stats.ActionPoints.GetValue() <= 0)
            return;

        character.ToggleAim();
    }
}
