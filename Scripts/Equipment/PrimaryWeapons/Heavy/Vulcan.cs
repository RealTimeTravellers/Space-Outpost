public partial class Vulcan : PrimaryWeapon
{
    public Vulcan()
    {
        Name = "Vulcan Rocket Launcher";
        Description = "Çok geniş bir alanda büyük hasar veren, mermi başına yüksek etki gücüne sahip bir roketatar.";
        PlayerType = PlayerType.Heavy;
        WeaponType = PrimaryWeaponType.Vulcan;
        MaxAmmoClip = 1;
        CurrentAmmoClip = MaxAmmoClip;
        Accuracy = 2;
        MinDamage = 2;
        MaxDamage = 6;
        Range = 20;
        Impact = 7;
        CritChance = 3;
    }
}