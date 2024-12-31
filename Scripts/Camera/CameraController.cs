using Godot;
using System;

public partial class CameraController : Node3D
{
    [Export] Raycaster raycaster;
    [Export] public Camera3D Camera {get; private set; }
    [Export] float cameraSpeed = 5f;
    [Export] float zoomSpeed = 0.1f;
    [Export] private float multiplier = 2f;

    public override void _Process(double delta)
    {
        ProcessCameraMovement((float) delta);
        SetMultiplier();
        base._Process(delta);
    }

    public override void _Input(InputEvent @event)
    {
        TacticalCameraZoom(@event);
        base._Input(@event);
    }

    private void SetMultiplier()
    {
        if (Input.IsActionPressed("Left Shift"))
            multiplier = 2;
        else
            multiplier = 1;
    }

    private void ProcessCameraMovement(float delta)
    {
        // TODO: Tween?

        if (CameraManager.Instance.AimingMode || CameraManager.Instance.TeamSelection) return;

        if(Input.IsActionPressed("Move Left"))
            this.Position += cameraSpeed * Vector3.Left * delta * multiplier;
        else if (Input.IsActionPressed("Move Right"))
            this.Position += cameraSpeed * Vector3.Right * delta * multiplier;
        
        if (Input.IsActionPressed("Move Forward"))
            this.Position += cameraSpeed * Vector3.Forward * delta * multiplier;
        else if (Input.IsActionPressed("Move Backward"))
            this.Position += cameraSpeed * Vector3.Back * delta * multiplier;
        
        if (!CameraManager.Instance.AimingMode)
            CameraManager.Instance.TacticalCameraPostion.Position = Position;
    }

    private void TacticalCameraZoom(InputEvent @event)
    {
        if (CameraManager.Instance.AimingMode || CameraManager.Instance.TeamSelection) return; // IDEA: maybe switch to fov zoom

        if(@event.IsAction("Zoom In"))
            Position += zoomSpeed * Vector3.Up * multiplier;
        else if (@event.IsAction("Zoom Out"))
            Position += zoomSpeed * Vector3.Down * multiplier;
        
        if (!CameraManager.Instance.AimingMode)
            CameraManager.Instance.TacticalCameraPostion.Position = Position;
    }
}
