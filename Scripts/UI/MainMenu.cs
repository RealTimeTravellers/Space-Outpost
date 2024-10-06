using Godot;
using System;

public partial class MainMenu : Control
{
	public void OnTestBattleButtonPressed()
	{
		GameManager.ChangeGameState(GameState.Menu, GameState.Battle);
	}
}
