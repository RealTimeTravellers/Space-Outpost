public partial class RangerStats : EnemyStats
{
    public RangerStats() : base()
    {
        UnitType = UnitType.Human;
        EnemyType = EnemyType.Ranger;
        Health.SetDefaultValue(5);
        Armor.SetDefaultValue(2);
        Accuracy.SetDefaultValue(80);
        MovementRange.SetDefaultValue(7);
        Morale.SetDefaultValue(22);
        ActionPoints.SetDefaultValue(3);
        Evasion.SetDefaultValue(25);
        CriticalHitChance.SetDefaultValue(40);
        Perception.SetDefaultValue(90);
    }
}