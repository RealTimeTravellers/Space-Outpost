using System;
using System.Collections.Generic;
using Godot;

public enum EnemyType
{
    Telepath,
    Creeper,
    Seperatist,
    Ranger,
    Rebel,
    Boss,
}

public partial class EnemyStats : UnitStats
{
    [Export] public EnemyType EnemyType { get; private set; }

    public EnemyStats(EnemyType enemyType, StatContainer statContainer) : base(statContainer)
    {
        EnemyType = enemyType;
    }
}