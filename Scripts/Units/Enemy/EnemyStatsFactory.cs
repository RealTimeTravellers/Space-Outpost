using System;
using System.Collections.Generic;

public static class EnemyStatsFactory
{
    public static StatContainer CreateStatsForEnemyType(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Telepath => new TelepathStats(),
            EnemyType.Creeper => new CreeperStats(),
            EnemyType.Seperatist => new SeperatistStats(),
            EnemyType.Ranger => new RangerStats(),
            EnemyType.Rebel => new RebelStats(),
            EnemyType.Boss => new BossStats(),
            _ => throw new ArgumentException($"Unknown enemy type: {enemyType}")
        };
    }

    public static List<StatContainer> GetAllEnemyStats()
    {
        return new List<StatContainer>
        {
            CreateStatsForEnemyType(EnemyType.Telepath),
            CreateStatsForEnemyType(EnemyType.Creeper),
            CreateStatsForEnemyType(EnemyType.Seperatist),
            CreateStatsForEnemyType(EnemyType.Ranger),
            CreateStatsForEnemyType(EnemyType.Rebel),
            CreateStatsForEnemyType(EnemyType.Boss)
        };
    }
}