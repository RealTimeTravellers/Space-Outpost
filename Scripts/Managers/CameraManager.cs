using Godot;
using System;

public partial class CameraManager : Node
{
    public static CameraManager Instance {get; private set;}
    [Export] public Camera3D mainCamera;
    [Export] public Node3D TacticalCameraPostion;
	[Export] public bool AimingMode { get; private set; } = false;
    

    private CameraManager()
    {
        Instance = this;
    }

    public static void ReturnCameraToTactical()
    {
        Instance.mainCamera.Transform = Instance.TacticalCameraPostion.GlobalTransform;
        Instance.AimingMode = false;
    }

    public static void MoveToShoulder(Character character)
    {
        Instance.mainCamera.Transform = character.ShoulderCamera.GlobalTransform;
        Instance.AimingMode = true;
    }
}
