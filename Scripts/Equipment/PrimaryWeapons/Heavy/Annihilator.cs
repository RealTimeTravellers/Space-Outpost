public partial class Annihilator : PrimaryWeapon
{
    public Annihilator()
    {
        Name = "Annihilator HMG";
        Description = "Daha küçük cephane kapasitesine sahip ancak her atışta büyük bir etki yaratan bir makineli tüfek.";
        PlayerType = PlayerType.Heavy;
        WeaponType = PrimaryWeaponType.Annihilator;
        AmmoClip = 2;
        Accuracy = 3;
        MinDamage = 3;
        Range = 15;
        MaxDamage = 8;
        Impact = 5;
        CritChance = 7;
    }
}
