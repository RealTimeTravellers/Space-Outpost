using Godot;

public partial class DroneShield : Accessory
{
    public DroneShield()
    {
        AccessoryType = AccessoryType.DroneShield;
        Name = "Drone Shield";
        Description = "Geçici olarak bir birimin etrafında koruyucu bir enerji kalkanı oluşturan yüksek teknolojili bir cihaz.";
        UsageCount = 1;
        IsPassive = false;
        Range = 10;
        IsPermanent = false;
        Effect = "%50 şans ile 2 tur boyunca hasarı %50 azaltır";
    }

    public override bool Use(Unit user, Unit target)
    {
        if (base.Use(user, target))
        {
            if (target != null && GD.Randf() <= 0.5f)
            {
                // Implement shield effect for 2 turns
            }
            return true;
        }
        return false;
    }

}