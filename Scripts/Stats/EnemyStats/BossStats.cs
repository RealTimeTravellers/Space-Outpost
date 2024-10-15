public partial class BossStats : EnemyStats
{
    public BossStats() : base()
    {
        EnemyType = EnemyType.Boss;
        Health.SetDefaultValue(10);
        Armor.SetDefaultValue(5);
        Accuracy.SetDefaultValue(85);
        MovementRange.SetDefaultValue(5);
        Morale.SetDefaultValue(10);
        ActionPoints.SetDefaultValue(3);
        Evasion.SetDefaultValue(30);
        CriticalHitChance.SetDefaultValue(50);
        Perception.SetDefaultValue(95);
    }
}