public partial class Suppressor : PrimaryWeapon
{
    public Suppressor()
    {
        Name = "Medic Suppressor MG";
        Description = "Susturucu özelliği sayesinde sessiz saldırılar yapabilir. Düşman hatlarını bozmak için idealdir.";
        PlayerType = PlayerType.Medic;
        PrimaryWeaponType = PrimaryWeaponType.Suppressor;
        MaxAmmoClip = 2;
        CurrentAmmoClip = MaxAmmoClip;
        Accuracy = 6;
        Range = 15;
        MinDamage = 3;
        MaxDamage = 4;
        Impact = 7;
        CritChance = 6;
    }
}