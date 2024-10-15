public partial class Thumper : PrimaryWeapon
{
    public Thumper()
    {
        Name = "Thumper High Impact Shotgun";
        Description = "Clipte az mermi bulunan (4 mermi), ancak her atışında büyük hasar ve geniş etki alanı sağlayan bir shotgun.";
        PlayerType = PlayerType.Engineer;
        AmmoClip = 3;
        Accuracy = 4;
        Range = 9;
        MinDamage = 3;
        MaxDamage = 7;
        Impact = 6;
        CritChance = 8;
    }
}