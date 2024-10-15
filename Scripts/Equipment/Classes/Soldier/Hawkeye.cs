public partial class Hawkeye : PrimaryWeapon
{
    public Hawkeye()
    {
        Name = "Hawkeye Marksman Rifle";
        Description = "Az mermisi olan (3 mermi), tek atışlı ve isabet oranı yüksek tek atışlı saldırı tüfeği.";
        PlayerType = PlayerType.Soldier;
        AmmoClip = 3;
        Accuracy = 15;
        MinDamage = 4;
        MaxDamage = 6;
        Impact = 5;
        CritChance = 15;
    }
}