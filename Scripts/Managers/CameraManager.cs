using Godot;
using System;

public partial class CameraManager : Node
{
    public static CameraManager Instance {get; private set;}
    [Export] public Camera3D mainCamera;

    private CameraManager()
    {
        Instance = this;
    }
}
