using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
    public static EnemyManager Instance {get; private set;}

    [Export] // Exported for testing
    public Godot.Collections.Array<Character> allEnemies;
    
    [Export] // Exported for testing
    public Godot.Collections.Array<Character> spottedEnemies;

    [Export]
    public Godot.Collections.Array<GridObject> coverGrids = new();

    private EnemyManager()
    {
        Instance = this;
    }
}
