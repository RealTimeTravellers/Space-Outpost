using Godot;
using System;

public partial class CameraManager : Node
{
    public static CameraManager Instance {get; private set;}
    [Export] public CameraController MainCameraSet { get; private set; }
    public Camera3D MainCamera { get; private set; }
    [Export] public Node3D TacticalCameraPostion;
	[Export] public bool AimingMode { get; private set; } = false;
	[Export] public bool AreaSelection { get; private set; } = false;
    

    private CameraManager()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        SetMainCamera();
        base._Ready();
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
