public partial class RebelStats : EnemyStats
{
    public RebelStats() : base()
    {
        UnitType = UnitType.Human;
        EnemyType = EnemyType.Rebel;
        Health.SetDefaultValue(6);
        Armor.SetDefaultValue(3);
        Accuracy.SetDefaultValue(70);
        MovementRange.SetDefaultValue(6);
        Morale.SetDefaultValue(8);
        ActionPoints.SetDefaultValue(2);
        Evasion.SetDefaultValue(20);
        CriticalHitChance.SetDefaultValue(30);
        Perception.SetDefaultValue(80);
    }
}