using Godot;

public partial class SecondaryWeapon : Equipment
{
    [Export] public int AmmoClip { get; set; }
    [Export] public int Accuracy { get; set; }
    [Export] public int Damage { get; set; }
    [Export] public int Impact { get; set; }

    public override void ApplyEffects(UnitStats stats)
    {
        stats.AddModifier("Accuracy", Accuracy);
        // Add other effects as needed
    }

    public override void RemoveEffects(UnitStats stats)
    {
        stats.RemoveModifier("Accuracy", Accuracy);
        // Remove other effects as needed
    }
}