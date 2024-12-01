public partial class Binoculars : Accessory
{
    public Binoculars()
    {
        AccessoryType = AccessoryType.Binoculars;
        Name = "Binoculars";
        Description = "Uzaktaki düşmanları ve stratejik noktaları tespit etmek için kullanılan bir dürbün.";
        UsageCount = 1;
        IsPassive = false;
        IsPermanent = true;
        Range = int.MaxValue;
        Effect = "Büyük bir alanı 2 tur boyunca görmeyi sağlar";
    }

    public override void ApplyEffects(UnitStats stats)
    {

    }

    public override void RemoveEffects(UnitStats stats)
    {

    }

    public override bool Use(Unit user, Unit target = null)
    {
        if (base.Use(user, target))
        {
            // Implement logic to reveal a large area for 2 turns
            return true;
        }
        return false;
    }
}