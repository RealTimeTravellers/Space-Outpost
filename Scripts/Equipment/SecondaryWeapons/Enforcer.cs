public partial class Enforcer : SecondaryWeapon
{
    public Enforcer()
    {
        Name = "Enforcer 1911";
        Description = "Klasik bir tabanca, isabet oranı yüksek ve orta seviyede hasar veren bir silah.";
        MaxAmmoClip = 3;
        CurrentAmmoClip = MaxAmmoClip;
        Accuracy = 8;
        MinDamage = 3;
        MaxDamage = 5;
        Impact = 4;
        CritChance = 10;
    }
}