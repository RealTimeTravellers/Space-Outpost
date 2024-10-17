using Godot;
using System;

public partial class Raycaster : Camera3D // random name idk
{

    [Export] private float rayLength = 300;

    public override void _Input(InputEvent @event)
    {
        SelectViaRaycast();
        base._Input(@event);
    }

    private void SelectViaRaycast()
    {
        if (Input.IsActionJustPressed("Select"))
        {
            var from =  CameraManager.Instance.mainCamera.ProjectRayOrigin(GetViewport().GetMousePosition());
            var to = from +  CameraManager.Instance.mainCamera.ProjectRayNormal(GetViewport().GetMousePosition()) * rayLength;

            CastHit hit = PhysicsCasts.CastLine(this, from, to, PhysicsCasts.GetCollisionMask(1) ,true);

            if(hit.NonEmpty)
            {
                GD.Print(hit.Collider.GetParent().Name);
                // select here
            }
            else
                GD.Print("No Hit");
        }
    }

}
