public partial class Phalanx : PrimaryWeapon
{
    public bool IsSteady { get; set; }

    public Phalanx()
    {
        Name = "Phalanx MK-IV";
        Description = "Kurulum gerektiren, menzili çok uzun olan ve steady özelliği ile ek isabet sağlayan bir sniper tüfeği.";
        PlayerType = PlayerType.Sniper;
        WeaponType = PrimaryWeaponType.Phalanx;
        AmmoClip = 1;
        Accuracy = 15;
        MinDamage = 4;
        MaxDamage = 6;
        Range = 30;
        Impact = 6;
        CritChance = 25;
    }

    public override void ApplyEffects(UnitStats stats)
    {
        base.ApplyEffects(stats);
        if (IsSteady)
        {
            stats.AddModifier("Accuracy", 5);
        }
    }

    public override void RemoveEffects(UnitStats stats)
    {
        base.RemoveEffects(stats);
        if (IsSteady)
        {
            stats.RemoveModifier("Accuracy", 5);
        }
    }
}