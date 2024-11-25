public partial class IncendiaryGrenade : Accessory
{
    public IncendiaryGrenade()
    {
        AccessoryType = AccessoryType.IncendiaryGrenade;
        Name = "Incendiary Grenade";
        Description = "Düşmanları yakmak için kullanılan bir yangın bombası. Etki alanındaki düşmanlar birkaç tur boyunca hasar alır.";
        UsageCount = 1;
        IsPassive = false;
        Range = 10;
        IsPermanent = false;
        Effect = "Düşmanlar 2 tur boyunca yanar ve her tur hasar alır";
    }

    public override bool Use(Unit user, Unit target)
    {
        if (base.Use(user, target))
        {
            // Implement burn effect for 2 turns
            return true;
        }
        return false;
    }
}