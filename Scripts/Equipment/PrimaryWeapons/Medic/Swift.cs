public partial class Swift : PrimaryWeapon
{
    public Swift()
    {
        Name = "Swift Healer's Burst Rifle";
        Description = "Hızlı ateş hızı ve yüksek etki gücü ile takım arkadaşlarını iyileştirirken düşmanlara karşı da etkili olabilir.";
        PlayerType = PlayerType.Medic;
        WeaponType = PrimaryWeaponType.Swift;
        MaxAmmoClip = 2;
        CurrentAmmoClip = MaxAmmoClip;
        Accuracy = 13;
        Range = 12;
        MinDamage = 4;
        MaxDamage = 6;
        Impact = 4;
        CritChance = 12;
    }
}