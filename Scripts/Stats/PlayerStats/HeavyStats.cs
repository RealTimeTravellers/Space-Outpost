public partial class HeavyStats : UnitStats
{
    public HeavyStats() : base()
    {
        Health.SetDefaultValue(8);
        Armor.SetDefaultValue(5);
        Accuracy.SetDefaultValue(60);
        MovementRange.SetDefaultValue(4);
        Morale.SetDefaultValue(20);
        ActionPoints.SetDefaultValue(2);
        Evasion.SetDefaultValue(8);
        CriticalHitChance.SetDefaultValue(35);
        Perception.SetDefaultValue(50);
    }
}