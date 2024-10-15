// Sniper Rifles
public partial class Devastator : PrimaryWeapon
{
    public Devastator()
    {
        Name = "Devastator .50";
        Description = "Clipte çok az mermi (2 mermi) olan, ancak her vuruşunda inanılmaz yüksek hasar veren bir sniper tüfeği.";
        PlayerType = PlayerType.Sniper;
        AmmoClip = 2;
        Accuracy = 15;
        MinDamage = 4;
        MaxDamage = 8;
        Impact = 7;
        CritChance = 20;
    }
}