using System;
using System.Collections.Generic;
using Godot;

public static class EnemyStatsFactory
{
    private static readonly Dictionary<EnemyType, string> ResourcePaths = new()
    {
        { EnemyType.Telepath, "res://Scripts/Stats/EnemyStats/TelepathStats.tres" },
        { EnemyType.Creeper, "res://Scripts/Stats/EnemyStats/CreeperStats.tres" },
        { EnemyType.Seperatist, "res://Scripts/Stats/EnemyStats/SeperatistStats.tres" },
        { EnemyType.Ranger, "res://Scripts/Stats/EnemyStats/RangerStats.tres" },
        { EnemyType.Rebel, "res://Scripts/Stats/EnemyStats/RebelStats.tres" },
        { EnemyType.Boss, "res://Scripts/Stats/EnemyStats/BossStats.tres" }
    };

    public static StatContainer CreateStatsForEnemyType(EnemyType enemyType)
    {
        var resourcePath = ResourcePaths[enemyType];
        var resource = GD.Load<Resource>(resourcePath);
        
        if (resource == null)
        {
            GD.PrintErr($"Could not load enemy stats resource: {resourcePath}");
            return null;
        }
        
        return resource as StatContainer;
    }

    public static List<StatContainer> GetAllEnemyStats()
    {
        var stats = new List<StatContainer>();
        foreach (var enemyType in ResourcePaths.Keys)
        {
            var stat = CreateStatsForEnemyType(enemyType);
            if (stat != null)
            {
                stats.Add(stat);
            }
        }
        return stats;
    }
}