using Godot;
using System;

public enum GameState
{
	Start,
	Menu,
	Loading,
	// GamePlay Modes will be added here
    TeamSelect,
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
	private Node mainMenuNode;
	[Export] private PackedScene[] gameScenes;
	public Node currentSceneRoot;

	public GameManager()
	{
		Instance = this;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

	private async void LoadMainMenu(Node scene)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		GetTree().Root.AddChild(scene);
	}

	private void SpawnGameScene()
	{
		GetTree().Root.AddChild(currentSceneRoot);
		GetTree().QueueDelete(mainMenuNode);
	}

	private void OnGameStateChanged(GameState current, GameState newState)
	{
		switch(newState)
		{
			case GameState.TeamSelect:
				currentSceneRoot = ResourceLoader.Load<PackedScene>(gameScenes[0].ResourcePath).Instantiate();
				SpawnGameScene();
				break;
			case GameState.Menu:
				mainMenuNode = ResourceLoader.Load<PackedScene>(mainMenuSubScene.ResourcePath).Instantiate();
				LoadMainMenu(mainMenuNode);

				if(currentSceneRoot != null)
					GetTree().QueueDelete(currentSceneRoot);	
				break;
			case GameState.Battle:
				currentSceneRoot = ResourceLoader.Load<PackedScene>(gameScenes[1].ResourcePath).Instantiate();
				SpawnGameScene();
				break;
			case GameState.End:
				// TODO: execute closing procedure, for future
				break;
		}
	}
}
