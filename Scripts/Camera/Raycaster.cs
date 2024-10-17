using Godot;
using System;

public partial class Raycaster : Node3D // random name idk
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

            CastHit hit = PhysicsCasts.CastLine(this, from, to, PhysicsCasts.GetCollisionMask(4, 5), true);

            if(hit.NonEmpty)
            {
                // character
                if (hit.Collider.CollisionLayer == 4)
                {
                    Character selected = hit.Collider as Character;
                    GridManager.Instance.selectedGrid = selected.currentGrid;
                    GridManager.Instance.selectedCharacter = selected;
                    GridManager.Instance.previousGrid = null;
                }
                else if (hit.Collider.CollisionLayer == 5) // grid
                {
                    GridManager.Instance.selectedGrid = hit.Collider.GetParent() as GridObject;

                    if (GridManager.Instance.previousGrid == null)
                        GridManager.Instance.previousGrid = GridManager.Instance.selectedCharacter.currentGrid;
                }
            }
            else
                GD.Print("No Hit");
        }
    }

}
