using Godot;
using System;

public partial class CameraManager : Node
{
    public static CameraManager Instance {get; private set;}
    [Export] public Camera3D mainCamera;
    [Export] public Node3D TacticalCameraPostion;
    

    private CameraManager()
    {
        Instance = this;
    }

    public static void ReturnCameraToTactical()
    {
        Instance.mainCamera.Transform = Instance.TacticalCameraPostion.GlobalTransform;
    }

    public static void MoveToShoulder(Character character)
    {
        Instance.mainCamera.Transform = character.ShoulderCamera.GlobalTransform;
    }
}
