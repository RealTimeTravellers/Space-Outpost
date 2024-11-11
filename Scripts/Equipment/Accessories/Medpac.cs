public partial class Medpac : Accessory
{
    public Medpac()
    {
        Name = "Medpac";
        Description = "Yaralı takım arkadaşlarını iyileştirmek için kullanılan taşınabilir bir ilk yardım kiti.";
        UsageCount = 1;
        IsPermanent = false;
        IsPassive = false;
        Range = 1;
        Effect = "4 HP iyileştirir";
    }

    public override bool Use(Unit user, Unit target)
    {
        if (base.Use(user, target))
        {
            if (target != null)
            {
                target.Stats.Health.IncreaseValue(4);
            }
            return true;
        }
        return false;
    }
}