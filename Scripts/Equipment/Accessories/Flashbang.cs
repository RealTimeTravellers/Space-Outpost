public partial class Flashbang : Accessory
{
    public Flashbang()
    {
        Name = "Flashbang";
        Description = "Düşmanları geçici olarak sersemleten ve onların saldırı yeteneklerini sınırlayan bir el bombası.";
        UsageCount = 1;
        IsPassive = false;
        UsageRange = 10;
        IsPermanent = false;
        Effect = "Düşmanları bir turluğuna stunlar/dazeler";
    }

    public override bool Use(Unit user, Unit target)
    {
        if (base.Use(user, target))
        {
            // Implement stunning logic here
            return true;
        }
        return false;
    }
}