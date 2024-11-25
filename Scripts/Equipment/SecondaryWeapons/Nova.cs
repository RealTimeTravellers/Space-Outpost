public partial class NovaPlasma : SecondaryWeapon
{
    public NovaPlasma()
    {
        Name = "Nova Plasma Pistol";
        Description = "Geleceğin silahı, yüksek enerji atışları yapan bir plazma tabancası. Yüksek hasar verir ancak isabet oranı düşüktür.";
        MaxAmmoClip = 2;
        CurrentAmmoClip = MaxAmmoClip;
        Accuracy = 4;
        MinDamage = 2;
        MaxDamage = 8;
        Impact = 7;
        CritChance = 12;
    }
}