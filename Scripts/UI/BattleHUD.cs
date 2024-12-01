using Godot;
using System;

public partial class BattleHUD : Control
{
    private static void OnMovePressed()
    {
        GridManager.Instance.selectedCharacter.Move(GridManager.Instance.selectedGrid);
    }

    private static void OnAttackModePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        CameraManager.MoveToShoulder(character);
        character.AttackMode();
    }

    private static void OnLeftArrowPressed()
    {
        GridManager.Instance.selectedCharacter.ChangeTarget(true);
    }

    private static void OnRigtArrowPressed()
    {
        GridManager.Instance.selectedCharacter.ChangeTarget();
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
