public partial class TelepathStats : EnemyStats
{
    public TelepathStats() : base()
    {
        UnitType = UnitType.Human;
        EnemyType = EnemyType.Telepath;
        Health.SetDefaultValue(6);
        Armor.SetDefaultValue(2);
        Accuracy.SetDefaultValue(75);
        MovementRange.SetDefaultValue(5);
        Morale.SetDefaultValue(7);
        ActionPoints.SetDefaultValue(2);
        Evasion.SetDefaultValue(20);
        CriticalHitChance.SetDefaultValue(30);
        Perception.SetDefaultValue(85);
    }
}
