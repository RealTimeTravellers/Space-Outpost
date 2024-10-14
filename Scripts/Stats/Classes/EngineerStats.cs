public class EngineerStats : UnitStats
{
    public EngineerStats()
    {
        // Engineer için özel varsayılan statlar
        Health.SetDefaultValue(6);
        Armor.SetDefaultValue(4); // Yüksek zırh
        Accuracy.SetDefaultValue(60); // Orta seviye nişan
        MovementRange.SetDefaultValue(5); 
        Morale.SetDefaultValue(6); 
        ActionPoints.SetDefaultValue(2); 
        Evasion.SetDefaultValue(10); // Orta seviye kaçınma
        CriticalHitChance.SetDefaultValue(30); // Kritik vuruş şansı düşük
    }
}