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

    private Transform3D _tacticalTransform;

    public override void _Ready()
    {
        InitializeAsync();
        // TODO: check if camera is ready.
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
    }

    public static void ReturnCameraToTactical()
    {
        Instance.MainCameraSet.GlobalTransform = Instance._tacticalTransform;
        Instance.AimingMode = false;
    }

    public static void MoveToShoulder(Character character)
    {
        Instance._tacticalTransform = Instance.MainCameraSet.GlobalTransform;
        Instance.MainCameraSet.GlobalTransform = character.ShoulderCamera.GlobalTransform;
        Instance.AimingMode = true;
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

}
