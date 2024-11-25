public partial class Thunderstrike : SecondaryWeapon
{
    public Thunderstrike()
    {
        Name = "Thunderstrike .50";
        Description = "Ağır vuran, sınırlı cephane kapasitesine sahip tabanca. Her atışta yüksek hasar verir ancak az sayıda mermi içerir.";
        MaxAmmoClip = 2;
        CurrentAmmoClip = MaxAmmoClip;
        Accuracy = 4;
        MinDamage = 4;
        MaxDamage = 5;
        Impact = 8;
        CritChance = 15;
    }
}