using Godot;

public partial class PrimaryWeapon : Equipment
{
    [Export] public PlayerType PlayerType { get; set; }
    [Export] public PrimaryWeaponType WeaponType { get; set; }
    [Export] public int AmmoClip { get; set; }
    [Export] public int Accuracy { get; set; }
    [Export] public int MaxDamage { get; set; }
    [Export] public int MinDamage { get; set; }
    [Export] public int Impact { get; set; }
    [Export] public int CritChance { get; set; }

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

    public int DealDamage(bool isCritical = false)
    {
        int damageDealt = GD.RandRange(MinDamage, MaxDamage);
        if (isCritical)
        {
            damageDealt *= 2; 
        }
        return damageDealt;
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