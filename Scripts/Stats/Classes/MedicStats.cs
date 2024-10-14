public class MedicStats : UnitStats
{
    public MedicStats()
    {
        // Medic için özel varsayılan statlar
        Health.SetDefaultValue(5); 
        Armor.SetDefaultValue(3);
        Accuracy.SetDefaultValue(50); // Düşük nişan
        MovementRange.SetDefaultValue(6); 
        Morale.SetDefaultValue(7); 
        ActionPoints.SetDefaultValue(2); 
        Evasion.SetDefaultValue(10); 
        CriticalHitChance.SetDefaultValue(20); // Düşük kritik vuruş şansı
    }
}