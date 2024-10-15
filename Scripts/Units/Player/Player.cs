using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public enum PlayerType
{
    Soldier,
    Sniper,
    Engineer,
    Medic,
    Heavy,
}
public partial class Player : Unit
{
    [Export]
    public PlayerType PlayerType { get; set; }

    public Player()
    {
        InitializeStats();  
    }

    protected override void InitializeStats()
    {
        Stats = CreateStatsForPlayerType(PlayerType);
        GD.Print("Player stats initialized");
    }

    private UnitStats CreateStatsForPlayerType(PlayerType type)
    {
        return type switch
        {
            PlayerType.Soldier => new SoldierStats(),
            PlayerType.Sniper => new SniperStats(),
            PlayerType.Engineer => new EngineerStats(),
            PlayerType.Medic => new MedicStats(),
            PlayerType.Heavy => new HeavyStats(),
            _ => new UnitStats() // Default case
        };
    }
}
