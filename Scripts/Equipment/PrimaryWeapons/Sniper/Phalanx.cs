using Godot;

public partial class Phalanx : PrimaryWeapon
{
    public bool IsSteady { get; set; }

    public Phalanx()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Sniper/PhalanxStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Sniper;
        PrimaryWeaponType = PrimaryWeaponType.Phalanx;
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