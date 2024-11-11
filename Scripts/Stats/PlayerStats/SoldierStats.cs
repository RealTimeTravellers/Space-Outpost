public partial class SoldierStats : UnitStats
{
    public SoldierStats() : base()
    {
        Health.SetDefaultValue(7);
        Armor.SetDefaultValue(3);
        Accuracy.SetDefaultValue(70);
        MovementRange.SetDefaultValue(7);
        Morale.SetDefaultValue(22);
        ActionPoints.SetDefaultValue(2);
        Evasion.SetDefaultValue(12);
        CriticalHitChance.SetDefaultValue(40);
        Perception.SetDefaultValue(60);
    }
}