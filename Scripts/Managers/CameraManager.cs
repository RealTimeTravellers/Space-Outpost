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
    }

    public static void ReturnCameraToTactical()
    {
        Instance.MainCameraSet.Transform = Instance.TacticalCameraPostion.GlobalTransform;
        Instance.AimingMode = false;
    }

    public static void MoveToShoulder(Character character)
    {
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
