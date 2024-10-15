public partial class Suppressor : PrimaryWeapon
{
    public Suppressor()
    {
        Name = "Medic Suppressor MG";
        Description = "Susturucu özelliği sayesinde sessiz saldırılar yapabilir. Düşman hatlarını bozmak için idealdir.";
        PlayerType = PlayerType.Medic;
        AmmoClip = 2;
        Accuracy = 6;
        MinDamage = 3;
        MaxDamage = 4;
        Impact = 7;
        CritChance = 6;
    }
}