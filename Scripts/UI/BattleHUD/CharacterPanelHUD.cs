using Godot;
using System;

public partial class CharacterPanelHUD : Node
{
	[Export] public Label NameLabel;
	[Export] public ProgressBar HealthBar;
	[Export] public Button NextCharacterButton;
	[Export] public Button PreviousCharacterButton;
	[Export] public Godot.Collections.Array<Button> CharacterButtons;

	
	private Raycaster raycaster;
	private int currentCharacterIndex = -1;
	private float cameraY;
	public override void _Ready()
	{
		GridManager.Instance.SelectionChanged += UpdateCharacterUI;
		cameraY = CameraManager.Instance.TacticalCameraPostion.Position.Y;
		ResetCharacterUI();
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		GridManager.Instance.SelectionChanged -= UpdateCharacterUI;
	}

	private void UpdateCharacterUI(GridObject gridObject)
	{
		if (gridObject == null) return;

		var selectedCharacter = GridManager.Instance.selectedCharacter;
		if (selectedCharacter == null) return;

		if (selectedCharacter.IsFriendly)
			DrawCharacterUI(selectedCharacter);
		else
			ResetCharacterUI();
		
	}

	private void DrawCharacterUI(Character selectedCharacter)
	{
		NameLabel.Text = selectedCharacter.Name;
		HealthBar.Value = selectedCharacter.Health;
		HealthBar.MaxValue = selectedCharacter.MaxHealth;
	}

	private void ResetCharacterUI()
	{
		currentCharacterIndex = -1;
		NameLabel.Text = "Select Character";
		HealthBar.Value = 0;
	}

	public void OnNextCharacter()
	{
		var playerCharacters = TurnManager.Instance.playerCharacters;
		if (playerCharacters.Count == 0) return;

		if (currentCharacterIndex < 0 || currentCharacterIndex >= playerCharacters.Count)
			currentCharacterIndex = 0;
		else
			currentCharacterIndex = (currentCharacterIndex + 1) % playerCharacters.Count;

		var nextCharacter = playerCharacters[currentCharacterIndex];
		if (nextCharacter == null) return;

		SelectCharacter(nextCharacter);
	}

	public void OnPreviousCharacter()
	{
		var playerCharacters = TurnManager.Instance.playerCharacters;
		if (playerCharacters.Count == 0) return;

		if (currentCharacterIndex < 0 || currentCharacterIndex >= playerCharacters.Count)
			currentCharacterIndex = 0;
		else
			currentCharacterIndex = (currentCharacterIndex - 1 + playerCharacters.Count) % playerCharacters.Count;

		var previousCharacter = playerCharacters[currentCharacterIndex];
		if (previousCharacter == null) return;

		SelectCharacter(previousCharacter);
	}

	public void OnCharacterButtonPressed(int index)
	{
		var playerCharacters = TurnManager.Instance.playerCharacters;
		if (playerCharacters.Count == 0 || index >= playerCharacters.Count) return;

		currentCharacterIndex = index;
		var selectedCharacter = playerCharacters[currentCharacterIndex];
		if (selectedCharacter == null) return;

		SelectCharacter(selectedCharacter);
	}

	public void SelectCharacter(Character character)
	{
		GridManager.Instance.selectedCharacter = character;
		GridManager.Instance.previousGrid = null;

		// 45 degrees camera angle, cameraY needs to be divided by sqrt(2) for z loc.
		CameraManager.Instance.TacticalCameraPostion.Position = new Vector3(character.Position.X, cameraY, character.Position.Z + cameraY/Mathf.Sqrt(2));
		CameraManager.Instance.MainCameraSet.raycaster.ChangeGridSelection(character.currentGrid);

		UpdateCharacterUI(character.currentGrid);
	}


}
