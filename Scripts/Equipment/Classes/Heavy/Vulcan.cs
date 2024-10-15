public partial class Vulcan : PrimaryWeapon
{
    public Vulcan()
    {
        Name = "Vulcan Rocket Launcher";
        Description = "Çok geniş bir alanda büyük hasar veren, mermi başına yüksek etki gücüne sahip bir roketatar.";
        PlayerType = PlayerType.Heavy;
        AmmoClip = 1;
        Accuracy = 2;
        MinDamage = 2;
        MaxDamage = 6;
        Impact = 7;
        CritChance = 3;
    }
}