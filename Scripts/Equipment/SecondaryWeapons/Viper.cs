public partial class Viper : SecondaryWeapon
{
    public Viper()
    {
        Name = "Viper 9mm";
        Description = "Hafif vuran, ancak hızlı ateş edebilen ve dengeli bir isabet oranına sahip tabanca. Mermi kapasitesi yüksektir.";
        AmmoClip = 4;
        Accuracy = 10;
        MinDamage = 2;
        MaxDamage = 4;
        Impact = 3;
        CritChance = 8;
    }
}