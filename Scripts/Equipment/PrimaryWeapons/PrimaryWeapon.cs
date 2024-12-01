using Godot;

public partial class PrimaryWeapon : Weapon
{
    [Export] public PlayerType PlayerType { get; set; }
    [Export] public PrimaryWeaponType PrimaryWeaponType { get; set; }

    public override void ApplyEffects(UnitStats stats)
    {
        stats.AddModifier("Accuracy", Accuracy);
        stats.AddModifier("CriticalHitChance", CritChance);
    }

    public override void RemoveEffects(UnitStats stats)
    {
        stats.RemoveModifier("Accuracy", Accuracy);
        stats.RemoveModifier("CriticalHitChance", CritChance);
    }

    public int DealImpact()
    {
        return Impact;
    }

    public bool IsInRange(int distance)
    {
        return distance <= Range;
    }
}