public partial class MedicStats : UnitStats
{
    public MedicStats() : base()
    {
        Health.SetDefaultValue(5);
        Armor.SetDefaultValue(3);
        Accuracy.SetDefaultValue(50);
        MovementRange.SetDefaultValue(6);
        Morale.SetDefaultValue(18);
        ActionPoints.SetDefaultValue(2);
        Evasion.SetDefaultValue(10);
        CriticalHitChance.SetDefaultValue(20);
        Perception.SetDefaultValue(70);
    }
}