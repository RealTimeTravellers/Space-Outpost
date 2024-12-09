using System;
using System.Collections.Generic;
using Godot;

public static class PlayerStatsFactory
{
    private static readonly Dictionary<PlayerType, string> ResourcePaths = new()
    {
        { PlayerType.Soldier, "res://Scripts/Stats/PlayerStats/SoldierStats.tres" },
        { PlayerType.Medic, "res://Scripts/Stats/PlayerStats/MedicStats.tres" },
        { PlayerType.Engineer, "res://Scripts/Stats/PlayerStats/EngineerStats.tres" },
        { PlayerType.Heavy, "res://Scripts/Stats/PlayerStats/HeavyStats.tres" },
        { PlayerType.Sniper, "res://Scripts/Stats/PlayerStats/SniperStats.tres" }
    };

    public static StatContainer CreateStatsForPlayerType(PlayerType playerType)
    {
        var resourcePath = ResourcePaths[playerType];
        var resource = GD.Load<Resource>(resourcePath);
        
        if (resource == null)
        {
            GD.PrintErr($"Could not load player stats resource: {resourcePath}");
            return null;
        }
        
        return resource as StatContainer;
    }

    public static List<StatContainer> GetAllPlayerStats()
    {
        var stats = new List<StatContainer>();
        foreach (var playerType in ResourcePaths.Keys)
        {
            var stat = CreateStatsForPlayerType(playerType);
            if (stat != null)
            {
                stats.Add(stat);
            }
        }
        return stats;
    }
}