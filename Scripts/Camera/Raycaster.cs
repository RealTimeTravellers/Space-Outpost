using Godot;
using BZ.Physics;

public partial class Raycaster : Node3D // random name idk
{

    public static bool MouseOverUI = false;
    [Export] private float rayLength = 300;
    

    public override void _Input(InputEvent @event)
    {
        if (MouseOverUI && (GameManager.Instance.gameState == GameState.InsideBuilding || GameManager.Instance.gameState == GameState.Desert))
            SelectViaRaycast();
        base._Input(@event);
    }

    public void ChangeGridSelection(GridObject gridObject)
    {
        GridManager.Instance.selectedGrid = gridObject;
        GridManager.Instance.SelectionChanged.Invoke(GridManager.Instance.selectedGrid);
    }

    private void SelectViaRaycast()
    {
        if (Input.IsActionJustPressed("Select"))
        {
            var from =  CameraManager.Instance.MainCamera.ProjectRayOrigin(GetViewport().GetMousePosition());
            var to = from +  CameraManager.Instance.MainCamera.ProjectRayNormal(GetViewport().GetMousePosition()) * rayLength;

            CastHit hit = this.CastLine3D(from, to, PhysicsHelper.GetCollisionMask(4, 5), true);

            if(hit.NonEmpty)
            {
                // character
                if (hit.Collider.CollisionLayer == PhysicsHelper.GetCollisionMask(4))
                {
                    Character selected = hit.Collider as Character;
                    GridManager.Instance.selectedCharacter = selected;
                    GridManager.Instance.previousGrid = null;
                    ChangeGridSelection(selected.currentGrid);
                }
                else if (hit.Collider.CollisionLayer == PhysicsHelper.GetCollisionMask(5)) // grid
                {
                    if((GridObject) hit.Collider.GetParent() == GridManager.Instance.selectedGrid) // deselect
                        ChangeGridSelection(null);
                    else // select
                        ChangeGridSelection(hit.Collider.GetParent() as GridObject);

                    if (GridManager.Instance.selectedCharacter == null) return;

                    if (GridManager.Instance.previousGrid == null)
                        GridManager.Instance.previousGrid = GridManager.Instance.selectedCharacter.currentGrid;
                }
            }
            else
                ChangeGridSelection(null);
        }
    }
}