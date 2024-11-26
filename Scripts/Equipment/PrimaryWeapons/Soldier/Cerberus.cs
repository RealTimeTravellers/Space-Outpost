public partial class Cerebus : PrimaryWeapon
{
    public Cerebus()
    {
        Name = "Cerebus Burst Rifle";
        Description = "Düşük isabet oranına sahip (4 mermi) ancak her ateşte üçlü burst atış yapan bir saldırı tüfeği.";
        PlayerType = PlayerType.Soldier;
        PrimaryWeaponType = PrimaryWeaponType.Cerberus;
        MaxAmmoClip = 4;
        CurrentAmmoClip = MaxAmmoClip;
        Accuracy = 13;
        Range = 15;
        MinDamage = 3;
        MaxDamage = 7;
        Impact = 6;
        CritChance = 10;
    }
}