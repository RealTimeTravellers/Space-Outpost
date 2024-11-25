using System;
using System.Collections.Generic;
using Godot;

public enum PlayerType
{
    Soldier,
    Sniper,
    Engineer,
    Medic,
    Heavy,
}

public partial class PlayerStats : UnitStats
{
    [Export] public PlayerType PlayerType { get; private set; }

    public PlayerStats(PlayerType playerType, StatContainer statContainer) : base(statContainer)
    {
        PlayerType = playerType;
        UnitType = UnitType.Human;
    }
}