public partial class Razorback : PrimaryWeapon
{
    public Razorback()
    {
        Name = "Razorback Dual Barrel Shotgun";
        Description = "Yüksek etki gücüne sahip çift namlulu bir shotgun. Yakın mesafede büyük hasar verir.";
        PlayerType = PlayerType.Engineer;
        WeaponType = PrimaryWeaponType.Razorback;
        AmmoClip = 2;
        Accuracy = 6;
        Range = 10;
        MinDamage = 2;
        MaxDamage = 7;
        Impact = 7;
        CritChance = 10;
    }
}
