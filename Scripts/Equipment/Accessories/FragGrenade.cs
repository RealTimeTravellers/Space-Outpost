public partial class FragGrenade : Accessory
{
    public FragGrenade()
    {
        AccessoryType = AccessoryType.FragGrenade;
        Name = "Frag Grenade";
        Description = "Alan etkili patlayıcı bir el bombası. Yakındaki düşmanlara hasar verir ve çevresel yıkım yaratır.";
        UsageCount = 1;
        IsPassive = false;
        Range = 10;
        IsPermanent = false;
        Effect = "Orta-yüksek hasar, küçük bir patlama alanı";
    }

    public override bool Use(Unit user, Unit target)
    {
        if (base.Use(user, target))
        {
            // Implement area damage logic here
            return true;
        }
        return false;
    }
}