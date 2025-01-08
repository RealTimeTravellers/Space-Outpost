using Godot;
using System;
using System.Threading.Tasks;
public partial class CameraManager : Node
{
    public static CameraManager Instance { get; set; }
    [Export] public CameraController MainCameraSet { get; private set; }
    public Camera3D MainCamera { get; private set; }
    [Export] public Node3D TacticalCameraPostion;
    [Export] public bool AimingMode { get; set; } = false;
    [Export] public bool AreaSelection { get; set; } = false;
    [Export] public bool TeamSelection { get; set; } = false;
    [Export] public float defaultCameraY { get; private set; }

    private Transform3D _tacticalTransform;

    public override void _Ready()
    {
        InitializeAsync();
        GameManager.GameStateChanged += OnGameStateChanged;
        // TODO: check if camera is ready.
    }

    public override void _ExitTree()
    {
        GameManager.GameStateChanged -= OnGameStateChanged;
    }

    private async void InitializeAsync()
    {
        if (Instance == null)
            Instance = this;
        else
            QueueFree();

        await SetMainCamera();
    }

    private async Task SetMainCamera()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        MainCamera = MainCameraSet.Camera;
        _tacticalTransform = TacticalCameraPostion.GlobalTransform;
        defaultCameraY = (int)TacticalCameraPostion.GlobalPosition.Y;
    }

    public static void ReturnCameraToTactical()
    {
        Instance.MainCameraSet.GlobalTransform = Instance._tacticalTransform;
        Instance.AimingMode = false;
    }

    public static void MoveToShoulder(Character character)
    {
        if (character.IsFriendly)
        {
            Instance._tacticalTransform = Instance.MainCameraSet.GlobalTransform;
            Instance.MainCameraSet.GlobalTransform = character.ShoulderCamera.GlobalTransform;
            Instance.AimingMode = true;
        }
    }

    public static void AreaSelectionMode()
    {
        ReturnCameraToTactical();
        Instance.AreaSelection = true;
    }

    public void OnAreaSelected()
    {
        // If i manage to do it through raycaster this may be needed.
    }

    private void OnGameStateChanged(GameState current, GameState newState)
    {
        if (newState == GameState.TeamSelect)
        {
            TeamSelection = true;
            MainCameraSet.GlobalPosition = new Vector3(0, 0f, 2.5f);
            MainCameraSet.LookAt(new Vector3(0, 2f, -2f));
        }
        else if (current == GameState.TeamSelect)
        {
            TeamSelection = false;
            MainCameraSet.GlobalTransform = _tacticalTransform;
        }
    }

}
