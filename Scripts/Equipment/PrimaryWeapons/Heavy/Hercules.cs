public partial class Hercules : PrimaryWeapon
{
    public Hercules()
    {
        Name = "Hercules HMG";
        Description = "Yüksek hasar veren ağır makineli tüfek. Kurşun sayısı fazla.";
        PlayerType = PlayerType.Heavy;
        WeaponType = PrimaryWeaponType.Hercules;
        MaxAmmoClip = 2;
        CurrentAmmoClip = MaxAmmoClip;
        Accuracy = 4;
        Range = 15;
        MinDamage = 4;
        MaxDamage = 8;
        Impact = 7;
        CritChance = 5;
    }
}