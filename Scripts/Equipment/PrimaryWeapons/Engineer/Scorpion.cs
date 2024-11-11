public partial class Scorpion : PrimaryWeapon
{
    public Scorpion()
    {
        Name = "Scorpion-12 Shotgun";
        Description = "Clipte 6 mermi taşıyan ve hızla ateş edebilen, orta hasar ve geniş menzile sahip bir shotgun.";
        PlayerType = PlayerType.Engineer;
        AmmoClip = 4;
        Accuracy = 5;
        Range = 12;
        MinDamage = 3;
        MaxDamage = 6;
        Impact = 5;
        CritChance = 5;
    }
}
