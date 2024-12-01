using Godot;
using System;

public partial class BattleHUD : Control
{

    private Character ChangeSelectedCharacter(bool toLeft)
    {
        return null;
    }

    private static void OnMovePressed()
    {
        GridManager.Instance.selectedCharacter.Move(GridManager.Instance.selectedGrid);
    }

    private static void OnFirePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (CameraManager.Instance.AimingMode)
            character.Attack(character.Target);
    }

    private static void OnAttackModePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        character.ToggleAim();
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

    private void OnMouseEnteredUI()
    {
        Raycaster.MouseOverUI = true;
    }

    private void OnMouseExitedUI()
    {
        Raycaster.MouseOverUI = false;
    }
    
}
