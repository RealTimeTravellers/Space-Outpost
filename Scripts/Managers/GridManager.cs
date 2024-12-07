using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GridManager : Node
{

    public static GridManager Instance {get; private set;}

    public Action<GridObject> SelectionChanged;

    [Export] public Resource gridData; // TODO: change to grid data resources type

    [Export] public PackedScene gridObjectSubscene;
    [Export] public Node3D gridParent;
    [Export] public Godot.Collections.Array<GridObject> gridList = new(); // not in particular order

    [Export] private int gridSize; // temp, will check from data

    [Export] public Character selectedCharacter;
    [Export] public GridObject selectedGrid;
    [Export] public GridObject previousGrid;

    /// <summary>
    /// Strictly a square
    /// </summary>
    [Export] public Godot.Collections.Array<Godot.Collections.Array<Node3D>> grids = new();

    private GridManager()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        CreateGrid();
        base._Ready();
    }

    private async void CreateOrAssignGrid()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        if (gridParent.GetChildCount() > 0) // if true grid is done by hand
            AssignGrid();
        else
            CreateGrid();
    }

    private void AssignGrid()
    {
        foreach (GridObject gridObject in gridParent.GetChildren().Cast<GridObject>())
            gridList.Add(gridObject);
    }

    private void CreateGrid()
    {
        for(int i = 0; i < gridSize; i++)
        {
            Godot.Collections.Array<Node3D> innerArray = new();
            for (int j = 0; j < gridSize; j++)
            {
                var gridObject = gridObjectSubscene.Instantiate<Node3D>();
                gridObject.Position = new Vector3(i - (gridSize / 2), 0, j - (gridSize / 2));
                gridParent.AddChild(gridObject);
                innerArray.Add(gridObject);
            }
            grids.Add(innerArray);
        }
    }
}
