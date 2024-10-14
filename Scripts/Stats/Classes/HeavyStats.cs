public class HeavyStats : UnitStats
{
    public HeavyStats()
    {
        // Heavy için özel varsayılan statlar
        Health.SetDefaultValue(8); // Yüksek sağlık
        Armor.SetDefaultValue(5); // Çok yüksek zırh
        Accuracy.SetDefaultValue(60); // Orta seviye nişan
        MovementRange.SetDefaultValue(4); // Düşük hareket kabiliyeti
        Morale.SetDefaultValue(7); 
        ActionPoints.SetDefaultValue(2); 
        Evasion.SetDefaultValue(8);  // Düşük kaçınma
        CriticalHitChance.SetDefaultValue(35); 
    }
}
