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
        GD.Print($"[EnemyManager] Shot reported at {shotLocation.GlobalPosition}");
    }
    
    public void ResetShotStatus()
    {
        ShotFired = false;
        LastShotGrid = null;
    }
}
