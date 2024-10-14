public class SniperStats : UnitStats
{
    public SniperStats()
    {
        // Sniper için özel varsayılan statlar
        Health.SetDefaultValue(5);
        Armor.SetDefaultValue(2);
        Accuracy.SetDefaultValue(85); // Sniper çok yüksek nişan oranına sahip
        MovementRange.SetDefaultValue(6); // Orta seviye hareket menzili
        Morale.SetDefaultValue(7); 
        ActionPoints.SetDefaultValue(2); 
        Evasion.SetDefaultValue(15); // Sniper hızlı hareket edebildiği için kaçınma oranı ortalama
        CriticalHitChance.SetDefaultValue(50); // Sniper kritik vuruş şansı yüksek
    }
}
