public partial class KevlarArmor : Accessory
{
    public KevlarArmor()
    {
        AccessoryType = AccessoryType.KevlarArmor;
        Name = "Kevlar Armor";
        Description = "Bir birimin savunmasını geçici olarak artıran zırh plakası. Gelen saldırılardan alınan hasarı azaltır.";
        UsageCount = 1;
        IsPassive = true;
        Range = 10;
        IsPermanent = true;
        Effect = "Armor artırır.";
    }

    public override void ApplyEffects(UnitStats stats)
    {
        if (IsPassive)
        {
            stats.AddModifier("Armor", 2);
        }
    }

    public override void RemoveEffects(UnitStats stats)
    {
        if (IsPassive)
        {
            stats.RemoveModifier("Armor", 2);
        }
    }
}