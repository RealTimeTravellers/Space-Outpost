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
                if (hit.Collider.CollisionLayer == PhysicsCasts.GetCollisionMask(4))
                {
                    Character selected = hit.Collider as Character;
                    GridManager.Instance.selectedGrid = selected.currentGrid;
                    GridManager.Instance.selectedCharacter = selected;
                    GridManager.Instance.previousGrid = null;
                    GridManager.Instance.SelectionChanged.Invoke(GridManager.Instance.selectedGrid);
                    //GD.Print(hit.Collider);
                }
                else if (hit.Collider.CollisionLayer == PhysicsCasts.GetCollisionMask(5)) // grid
                {
                    GridManager.Instance.selectedGrid = hit.Collider.GetParent() as GridObject;

                    GD.Print(hit.Collider.GetParent());

                    GridManager.Instance.SelectionChanged.Invoke(GridManager.Instance.selectedGrid);

                    if (GridManager.Instance.selectedCharacter == null) return;

                    if (GridManager.Instance.previousGrid == null)
                        GridManager.Instance.previousGrid = GridManager.Instance.selectedCharacter.currentGrid;

                }
            }
            else
                GridManager.Instance.SelectionChanged.Invoke(null);
        }
    }

}
