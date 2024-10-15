public partial class Swift : PrimaryWeapon
{
    public Swift()
    {
        Name = "Swift Healer's Burst Rifle";
        Description = "Hızlı ateş hızı ve yüksek etki gücü ile takım arkadaşlarını iyileştirirken düşmanlara karşı da etkili olabilir.";
        PlayerType = PlayerType.Medic;
        AmmoClip = 2;
        Accuracy = 13;
        MinDamage = 4;
        MaxDamage = 6;
        Impact = 4;
        CritChance = 12;
    }
}