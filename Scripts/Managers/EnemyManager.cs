using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
    public static EnemyManager Instance {get; private set;}

    [Export] public Godot.Collections.Array<GridObject> coverGrids = new(); // not in particular order

    [Export] // Exported for testing
    public Godot.Collections.Array<Character> allEnemies;
    
    [Export] // Exported for testing
    public Godot.Collections.Array<Character> spottedEnemies;
    public Dictionary<GridObject, Character> OccupiedCovers { get; private set; } = new();
    private Dictionary<GridObject, bool> targetedGrids = new Dictionary<GridObject, bool>();


    public bool ShotFired { get; private set; }
    public GridObject LastShotGrid { get; private set; }

    private EnemyManager()
    {
        Instance = this;
    }

    public void ReportShotFired(GridObject shotLocation)
    {
        ShotFired = true;
        LastShotGrid = shotLocation;
    }
    
    public void ResetShotStatus()
    {
        ShotFired = false;
        LastShotGrid = null;
    }

    public void OnEnemyDeath(Character enemy)
    {
        allEnemies.Remove(enemy);
        spottedEnemies.Remove(enemy);

        // stops combat loop so music can end
        if (allEnemies.Count <= 0)
            AudioManager.Instance.combatEnded = true;
    }

    public void RegisterCoverOccupation(GridObject cover, Character character)
    {
        if (!OccupiedCovers.ContainsKey(cover))
            OccupiedCovers[cover] = character;
    }

    public void UnregisterCoverOccupation(GridObject cover)
    {
        if (OccupiedCovers.ContainsKey(cover))
            OccupiedCovers.Remove(cover);
    }

    public bool IsGridTargeted(GridObject grid)
    {
        return targetedGrids.ContainsKey(grid);
    }

    public void RegisterTargetGrid(GridObject grid)
    {
        if (!targetedGrids.ContainsKey(grid))
            targetedGrids[grid] = true;
    }

    public void UnregisterTargetGrid(GridObject grid)
    {
        if (targetedGrids.ContainsKey(grid))
            targetedGrids.Remove(grid);
    }
}