using Godot;
using System;

public partial class CameraController : Node3D
{
    [Export] Raycaster raycaster;
    [Export] public Camera3D Camera {get; private set; }
    [Export] float cameraSpeed = 5f;
    [Export] float zoomSpeed = 0.1f;

    public override void _Process(double delta)
    {
        ProcessCameraMovement((float) delta);
        base._Process(delta);
    }

    public override void _Input(InputEvent @event)
    {
        TacticalCameraZoom(@event);
        base._Input(@event);
    }

    private void ProcessCameraMovement(float delta)
    {
        // TODO: Tween?

        if (CameraManager.Instance.AimingMode) return;

        if(Input.IsActionPressed("Move Left"))
            this.Position += cameraSpeed * Vector3.Left * delta;
        else if (Input.IsActionPressed("Move Right"))
            this.Position += cameraSpeed * Vector3.Right * delta;
        
        if (Input.IsActionPressed("Move Forward"))
            this.Position += cameraSpeed * Vector3.Forward * delta;
        else if (Input.IsActionPressed("Move Backward"))
            this.Position += cameraSpeed * Vector3.Back * delta;
        
        if (!CameraManager.Instance.AimingMode)
            CameraManager.Instance.TacticalCameraPostion.Position = Position;
    }

    private void TacticalCameraZoom(InputEvent @event)
    {
        if (CameraManager.Instance.AimingMode) return; // IDEA: maybe switch to fov zoom

        if(@event.IsAction("Zoom In"))
            Position += zoomSpeed * Vector3.Up;
        else if (@event.IsAction("Zoom Out"))
            Position += zoomSpeed * Vector3.Down;
        
        if (!CameraManager.Instance.AimingMode)
            CameraManager.Instance.TacticalCameraPostion.Position = Position;
    }
}
