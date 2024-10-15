public partial class SniperStats : UnitStats
{
    public SniperStats() : base()
    {
        Health.SetDefaultValue(5);
        Armor.SetDefaultValue(2);
        Accuracy.SetDefaultValue(85);
        MovementRange.SetDefaultValue(6);
        Morale.SetDefaultValue(7);
        ActionPoints.SetDefaultValue(2);
        Evasion.SetDefaultValue(15);
        CriticalHitChance.SetDefaultValue(50);
        Perception.SetDefaultValue(90);
    }
}