public partial class Falcon : SecondaryWeapon
{
    public Falcon()
    {
        Name = "Falcon Compact";
        Description = "Dengeli bir tabanca, orta menzil ve hasar sunar. Hem cephane kapasitesi hem de isabet oranı dengelidir.";
        AmmoClip = 3;
        Accuracy = 8;
        MinDamage = 3;
        MaxDamage = 6;
        Impact = 5;
        CritChance = 10;
    }
}