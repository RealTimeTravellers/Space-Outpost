public class SoldierStats : UnitStats
{
    public SoldierStats()
    {
        // Soldier için özel varsayılan statlar
        Health.SetDefaultValue(7); // Yüksek sağlık
        Armor.SetDefaultValue(3);
        Accuracy.SetDefaultValue(70); // Orta seviye nişan
        MovementRange.SetDefaultValue(7); // Hızlı hareket kabiliyeti
        Morale.SetDefaultValue(8); // Yüksek moral
        ActionPoints.SetDefaultValue(3); // Ekstra aksiyon puanı
        Evasion.SetDefaultValue(12); // Orta seviye kaçınma
        CriticalHitChance.SetDefaultValue(40); 
    }
}