public partial class Vanguard : PrimaryWeapon
{
    public Vanguard()
    {
        Name = "Vanguard SR-30";
        Description = "Çok sayıda mermi taşıyabilen (4 mermi), ancak isabet oranı daha düşük olan bir sniper tüfeği.";
        PlayerType = PlayerType.Sniper;
        WeaponType = PrimaryWeaponType.Vanguard;
        AmmoClip = 4;
        Accuracy = 10;
        MinDamage = 3;
        Range = 25;
        MaxDamage = 6;
        Impact = 5;
        CritChance = 15;
    }
}
