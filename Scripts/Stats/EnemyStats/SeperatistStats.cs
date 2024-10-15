public partial class SeperatistStats : EnemyStats
{
    public SeperatistStats() : base()
    {
        UnitType = UnitType.Human;
        EnemyType = EnemyType.Seperatist;
        Health.SetDefaultValue(7);
        Armor.SetDefaultValue(3);
        Accuracy.SetDefaultValue(70);
        MovementRange.SetDefaultValue(5);
        Morale.SetDefaultValue(6);
        ActionPoints.SetDefaultValue(2);
        Evasion.SetDefaultValue(15);
        CriticalHitChance.SetDefaultValue(35);
        Perception.SetDefaultValue(75);
    }
}