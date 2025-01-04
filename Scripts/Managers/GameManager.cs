using Godot;
using System;

public enum GameState
{
	Start,
	Menu,
	Loading,
	// GamePlay Modes will be added here
    TeamSelect,
	MissionSelect,
    Battle,
    // GamePlay Modes End
	Settings,
	End
}

public partial class GameManager : Node
{
	public static GameManager Instance {get; private set;}

	public GameState gameState {get; private set;} = GameState.Start;
	public static event Action<GameState, GameState> GameStateChanged;

	[Export] public Settings settings;
	[Export] private PackedScene mainMenuSubScene;
	[Export] private PackedScene settingsSubScene;
	[Export] private PackedScene endScreenSubScene;
	private Node mainMenuNode;
	private Node settingsNode;
	private Node endScreenNode;
	public event Action<bool> OnSettingsVisibilityChanged;
	[Export] private PackedScene[] gameScenes;

	public Node currentSceneRoot;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		Initialize();
		StartGame();
	}

	// NOTE: TEMP FOR DEBUG
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("End Game"))
		{
			ChangeGameState(gameState, GameState.Menu);
		}
	}

	public  static void ChangeGameState(GameState currentState, GameState newState)
	{
		Instance.gameState = newState;
		GameStateChanged.Invoke(currentState, newState); // can never be null, if its null something wrong
	}

	private void StartGame()
	{
		ChangeGameState(gameState, GameState.Menu);
	}

	private void Initialize()
	{
		gameState = GameState.Menu;
		GameStateChanged += OnGameStateChanged;
	}

	public override void _ExitTree()
	{
		GameStateChanged -= OnGameStateChanged;
	}

	public async void LoadEndScreen(bool isVictory)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		endScreenNode = ResourceLoader.Load<PackedScene>(endScreenSubScene.ResourcePath).Instantiate();
		endScreenNode.GetNode<EndScreenGUI>(".").InitEndScreen(isVictory);
		GetTree().Root.AddChild(endScreenNode);
	}

	private async void LoadMainMenu(Node scene)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		GetTree().Root.AddChild(scene);
	}

	private async void LoadSettingsMenu(Node scene)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		GetTree().Root.AddChild(scene);
	}

	public void ToggleSettingsPanel(bool show)
	{
		if (show && settingsNode == null)
		{
			settingsNode = settingsSubScene.Instantiate<Control>();
			LoadSettingsMenu(settingsNode);
		}
		else if (!show && settingsNode != null)
		{
			settingsNode.QueueFree();
			settingsNode = null;
		}

		OnSettingsVisibilityChanged?.Invoke(show);
	}

	private void SpawnGameScene()
	{
		GetTree().Root.AddChild(currentSceneRoot);

		if(mainMenuNode != null && mainMenuNode.IsInsideTree())
		{
			mainMenuNode.QueueFree();
			mainMenuNode = null;
		}
	}

	private async void OnGameStateChanged(GameState current, GameState newState)
	{
		Node newScene = null;

		// Need to wait for all scenes.
		if(currentSceneRoot != null)
		{
			GetTree().QueueDelete(currentSceneRoot);
			currentSceneRoot = null;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}

		// Load new scene based on new state.
		switch(newState)
		{
			case GameState.TeamSelect:    
				newScene = ResourceLoader.Load<PackedScene>(gameScenes[0].ResourcePath).Instantiate();
				break;

			case GameState.MissionSelect:
				newScene = ResourceLoader.Load<PackedScene>(gameScenes[2].ResourcePath).Instantiate();
				break;

			case GameState.Menu:
				mainMenuNode = ResourceLoader.Load<PackedScene>(mainMenuSubScene.ResourcePath).Instantiate();
				LoadMainMenu(mainMenuNode);
				ToggleSettingsPanel(false);
				break;
				
			case GameState.Battle:
				newScene = ResourceLoader.Load<PackedScene>(gameScenes[1].ResourcePath).Instantiate();
				break;

			case GameState.End:
				newScene = ResourceLoader.Load<PackedScene>(gameScenes[3].ResourcePath).Instantiate();
				break;
		}

		// Set new scene.
		if (newScene != null)
		{
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			currentSceneRoot = newScene;
			SpawnGameScene();
		}
	}
}
