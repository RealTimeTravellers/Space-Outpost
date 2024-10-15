public partial class TitanAssaultRifle : PrimaryWeapon
{
    public TitanAssaultRifle()
    {
        Name = "Titan Assault Rifle";
        Description = "Yüksek hasar veren, tek atışta düşmanı etkisiz hale getirebilen bir saldırı tüfeği.";
        PlayerType = PlayerType.Soldier;
        AmmoClip = 5;
        Accuracy = 11;
        MinDamage = 3;
        MaxDamage = 7;
        Impact = 7;
        CritChance = 12;
        Range = 20;
    }
}