public partial class CreeperStats : EnemyStats
{
    public CreeperStats() : base()
    {
        EnemyType = EnemyType.Creeper;
        Health.SetDefaultValue(8);
        Armor.SetDefaultValue(4);
        Accuracy.SetDefaultValue(65);
        MovementRange.SetDefaultValue(6);
        Morale.SetDefaultValue(8);
        ActionPoints.SetDefaultValue(3);
        Evasion.SetDefaultValue(10);
        CriticalHitChance.SetDefaultValue(25);
        Perception.SetDefaultValue(70);
    }
}