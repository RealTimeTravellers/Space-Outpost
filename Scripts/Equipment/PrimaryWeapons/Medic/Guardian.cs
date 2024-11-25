public partial class GuardianLight : PrimaryWeapon
{
    public GuardianLight()
    {
        Name = "Guardian Light Machine Gun";
        Description = "Dengeli bir silah olup, sürekli ateşleme kapasitesi sayesinde savunma ve destek görevlerinde etkilidir.";
        PlayerType = PlayerType.Medic;
        WeaponType = PrimaryWeaponType.Guardian;
        AmmoClip = 3;
        Accuracy = 12;
        Range = 12;
        MinDamage = 3;
        MaxDamage = 5;
        Impact = 4;
        CritChance = 10;
    }
}