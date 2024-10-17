using Godot;
using System;

public partial class Character : CharacterBody3D // don't really know why is this character body
{
    public GridObject currentGrid = null;

    [Export] public bool move = false; // temp for test only

    public override void _Process(double delta)
    {
        if (move) // test
        {
            move = !move;
            GlobalPosition = GridManager.Instance.selectedGrid.GlobalPosition;
        }
        base._Process(delta);
    }
}
