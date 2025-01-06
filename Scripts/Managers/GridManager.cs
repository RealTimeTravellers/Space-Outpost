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

    [Export] private int gridSize = 0; // temp, will check from data

    [Export] public Character selectedCharacter;
    [Export] public GridObject selectedGrid;
    [Export] public GridObject previousGrid;

    /// <summary>
    /// Strictly a square
    /// </summary>
    [Export] public Godot.Collections.Array<Godot.Collections.Array<GridObject>> grids = new();

    private GridManager()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        CreateOrAssignGrid();
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
        {
            gridList.Add(gridObject);
        }

    }

    private void CreateGrid()
    {
        for(int i = 0; i < gridSize; i++)
        {
            
            Godot.Collections.Array<GridObject> innerArray = new();
            for (int j = 0; j < gridSize; j++)
            {
                var gridObject = gridObjectSubscene.Instantiate<GridObject>();
                gridObject.Position = new Vector3(i - (gridSize / 2), 0, j - (gridSize / 2));
                gridParent.AddChild(gridObject);
                innerArray.Add(gridObject);
            }
            grids.Add(innerArray);
        }
    }

    public GridObject GetGridObjectFromWorldPosition(Vector3 worldPosition)
    {
        // Her grid 1 birim, direkt yuvarla
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(worldPosition.X),
            0,
            Mathf.Round(worldPosition.Z)
        );

        float tolerance = 0.8f;

        // Her bir satırı dön
        foreach (var grid in gridList)
        {
            if (Mathf.Abs(grid.GlobalPosition.X - snappedPosition.X) < tolerance &&
                Mathf.Abs(grid.GlobalPosition.Z - snappedPosition.Z) < tolerance)
            {
                return grid;
            }            
        }

        return null;
    }

    public Godot.Collections.Array<GridObject> GetNeighborGrids(GridObject grid, int radius = 1)
    {
        var neighbors = new Godot.Collections.Array<GridObject>();
        var gridPos = grid.GlobalPosition;
        
        // Check all 4 directions
        Vector3[] offsets = {
            new Vector3(1, 0, 0),   
            new Vector3(-1, 0, 0), 
            new Vector3(0, 0, 1),   
            new Vector3(0, 0, -1),  
        };

        foreach (var offset in offsets)
        {
            var checkPos = gridPos + offset;
            var neighborGrid = GetGridObjectFromWorldPosition(checkPos);
            if (neighborGrid != null)
            {
                neighbors.Add(neighborGrid);
            }
        }
        
        return neighbors;
    }

    public bool IsValidGrid(Vector3 position)
    {
        var grid = GetGridObjectFromWorldPosition(position);
        return grid != null && !grid.IsOccupied && !grid.IsBlocked;
    }

    public GridObject GetClosestGrid(Vector3 position)
    {
        return gridList
            .OrderBy(g => g.GlobalPosition.DistanceTo(position))
            .FirstOrDefault();
    }

    public List<GridObject> GetPathToTarget(GridObject start, GridObject target, int maxSteps = 10)
    {
        var path = new List<GridObject>();
        var current = start;
        var steps = 0;
        
        while (current != target && steps < maxSteps)
        {
            var nextGrid = GetNextGridInPath(current, target);
            if (nextGrid == null) break;
            
            path.Add(nextGrid);
            current = nextGrid;
            steps++;
        }
        
        return path;
    }

    public GridObject FindAlternativeGrid(GridObject targetGrid, Vector3 fromPosition)
    {
        var neighbors = GetNeighborGrids(targetGrid)
            .Where(g => !g.IsOccupied && !g.IsBlocked && !EnemyManager.Instance.IsGridTargeted(g))
            .OrderBy(g => g.GlobalPosition.DistanceTo(fromPosition));
        
        return neighbors.FirstOrDefault();
    }

    private GridObject GetNextGridInPath(GridObject current, GridObject target)
    {
        var directionToTarget = (target.GlobalPosition - current.GlobalPosition).Normalized();
        var nextPos = current.GlobalPosition + new Vector3(
            Mathf.Round(directionToTarget.X),
            0,
            Mathf.Round(directionToTarget.Z)
        );
        
        var nextGrid = GetGridObjectFromWorldPosition(nextPos);
        if (nextGrid != null && !nextGrid.IsOccupied && !nextGrid.IsBlocked)
            return nextGrid;
            
        return FindAlternativeGrid(current, target.GlobalPosition);
    }
}
