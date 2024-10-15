public partial class Cerebus : PrimaryWeapon
{
    public Cerebus()
    {
        Name = "Cerebus Burst Rifle";
        Description = "Düşük isabet oranına sahip (4 mermi) ancak her ateşte üçlü burst atış yapan bir saldırı tüfeği.";
        PlayerType = PlayerType.Soldier;
        AmmoClip = 4;
        Accuracy = 13;
        MinDamage = 3;
        MaxDamage = 7;
        Impact = 6;
        CritChance = 10;
    }
}