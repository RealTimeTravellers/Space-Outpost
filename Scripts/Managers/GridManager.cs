using Godot;
using System;

public partial class GridManager : Node
{

    public static GridManager Instance {get; private set;}
    [Export] public Resource gridData; // TODO: change to grid data resources type

    [Export] public PackedScene gridObjectSubscene;
    [Export] public Node3D gridParent;

    [Export] private int gridSize; // temp, will check from data

    public Character selectedCharacter; // currently Node3D will change it to Character.cs or smth
    public Node3D selectedGrid; // currently Node3D will change it to grid.cs or smth
    public Node3D previousGrid; // currently Node3D will change it to grid.cs or smth

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
