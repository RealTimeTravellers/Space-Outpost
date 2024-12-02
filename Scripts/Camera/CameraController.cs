using Godot;
using System;

public partial class CameraController : Node3D
{
    [Export] Raycaster raycaster;
    [Export] public Camera3D Camera {get; private set; }
    [Export] float cameraSpeed = 0.1f;

    public override void _Process(double delta)
    {
        ProcessCameraMovement();
        base._Process(delta);
    }

    public override void _Input(InputEvent @event)
    {
        TacticalCameraZoom(@event);
        base._Input(@event);
    }

    private void ProcessCameraMovement()
    {
        // TODO: Tween?

        if(Input.IsActionPressed("Move Left"))
            this.Position += cameraSpeed * Vector3.Left;
        else if (Input.IsActionPressed("Move Right"))
            this.Position += cameraSpeed * Vector3.Right;
        
        if (Input.IsActionPressed("Move Forward"))
            this.Position += cameraSpeed * Vector3.Forward;
        else if (Input.IsActionPressed("Move Backward"))
            this.Position += cameraSpeed * Vector3.Back;
    }

    private void TacticalCameraZoom(InputEvent @event)
    {
        if(@event.IsAction("Zoom In"))
            Position += cameraSpeed * Vector3.Up;
        else if (@event.IsAction("Zoom Out"))
            Position += cameraSpeed * Vector3.Down;
    }
}
