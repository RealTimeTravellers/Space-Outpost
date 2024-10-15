public partial class SoldierStats : UnitStats
{
    public SoldierStats() : base()
    {
        Health.SetDefaultValue(7);
        Armor.SetDefaultValue(3);
        Accuracy.SetDefaultValue(70);
        MovementRange.SetDefaultValue(7);
        Morale.SetDefaultValue(8);
        ActionPoints.SetDefaultValue(3);
        Evasion.SetDefaultValue(12);
        CriticalHitChance.SetDefaultValue(40);
        Perception.SetDefaultValue(60);
    }
}