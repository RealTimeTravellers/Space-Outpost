using Godot;
using System;

public partial class BattleHUD : Control
{
    private static void OnMovePressed()
    {
        GridManager.Instance.selectedCharacter.Move(GridManager.Instance.selectedGrid);
    }

    private static void OnFirePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        if (character.InAttackMode)
            character.Attack(character.Target);
    }

    private static void OnAttackModePressed()
    {
        Character character = GridManager.Instance.selectedCharacter;
        character.ToggleAttackMode();
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
