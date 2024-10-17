using Godot;
using System;

public partial class GridObject : Node3D
{
    [Export] private CoverType coverType = CoverType.None;
    [Export] public Godot.Collections.Array<bool> coverDirection = new Godot.Collections.Array<bool>{ false, false, false, false };
}
