using Godot;

public partial class EnemyStats : UnitStats
{
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Creeper;

    public EnemyStats() : base()
    {
        UnitType = UnitType.Alien;
    }

    public UnitStats CreateStatsForEnemyType(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Telepath => new TelepathStats(),
            EnemyType.Creeper => new CreeperStats(),
            EnemyType.Seperatist => new MedicStats(),
            EnemyType.Ranger => new RangerStats(),
            EnemyType.Rebel => new RebelStats(),
            EnemyType.Boss => new BossStats(),
            _ => new UnitStats() // Default case
        };
    }
}