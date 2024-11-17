using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
    public static EnemyManager Instance {get; private set;}

    public List<Character> allEnemies;
    public List<Character> spottedEnemies;

    private EnemyManager()
    {
        Instance = this;
    }
}
