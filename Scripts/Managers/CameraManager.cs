using Godot;
using System;

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
        if (Instance == null)
            Instance = this;
        else
            QueueFree();
            
        SetMainCamera();
    }

    private async void SetMainCamera()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        MainCamera = MainCameraSet.Camera;
        _tacticalTransform = TacticalCameraPostion.GlobalTransform;
    }

    public static void ReturnCameraToTactical()
    {
        Instance.MainCameraSet.Transform = Instance._tacticalTransform;
        Instance.AimingMode = false;
    }

    public static void MoveToShoulder(Character character)
    {
        var shoulderTransform = character.ShoulderCamera.GlobalTransform;
        Instance._tacticalTransform.Origin.X = shoulderTransform.Origin.X - Instance._tacticalTransform.Origin.Y * 0.25f;
        Instance._tacticalTransform.Origin.Z = shoulderTransform.Origin.Z + Instance._tacticalTransform.Origin.Y * 0.5f;

        Instance.MainCameraSet.Transform = character.ShoulderCamera.GlobalTransform;
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
