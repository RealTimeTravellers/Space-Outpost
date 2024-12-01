using Godot;

public abstract partial class Equipment : Resource
{
	[Export] public string Name { get; set; }
	[Export] public string Description { get; set; }
	[Export] public int Range { get; set; }
	[Export] public Texture2D Icon { get; set; }

	public abstract void ApplyEffects(UnitStats stats);
	public abstract void RemoveEffects(UnitStats stats);
}
