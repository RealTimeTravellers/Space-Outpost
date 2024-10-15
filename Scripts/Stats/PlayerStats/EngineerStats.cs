public partial class EngineerStats : UnitStats
{
    public EngineerStats() : base()
    {
        Health.SetDefaultValue(6);
        Armor.SetDefaultValue(4);
        Accuracy.SetDefaultValue(60);
        MovementRange.SetDefaultValue(5);
        Morale.SetDefaultValue(6);
        ActionPoints.SetDefaultValue(2);
        Evasion.SetDefaultValue(10);
        CriticalHitChance.SetDefaultValue(30);
        Perception.SetDefaultValue(80);
    }
}