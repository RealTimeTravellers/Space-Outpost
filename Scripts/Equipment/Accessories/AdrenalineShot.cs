public partial class AdrenalineShot : Accessory
{
    public AdrenalineShot()
    {
        Name = "Adrenaline Shot";
        Description = "Bir birime geçici bir enerji artışı sağlayan enjektör. birimin hızını ve saldırı gücünü artırır.";
        UsageCount = 1;
        IsPassive = false;
        Range = 1;
        IsPermanent = false;
        Effect = "Morali düşük birimleri toparlar";
    }

    public override bool Use(Unit user, Unit target)
    {
        if (base.Use(user, target))
        {
            if (target != null)
            {
                target.Stats.Morale.IncreaseValue(4);
                // Implement temporary speed and attack boost
            }
            return true;
        }
        return false;
    }
}