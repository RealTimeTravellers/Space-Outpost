using Godot;

public partial class Accessory : Equipment
{
    [Export] public int UsageCount { get; set; }

    public override void ApplyEffects(UnitStats stats)
    {
        // Apply accessory-specific effects
    }

    public override void RemoveEffects(UnitStats stats)
    {
        // Remove accessory-specific effects
    }
}