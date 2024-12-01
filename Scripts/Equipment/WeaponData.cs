using Godot;

public partial class WeaponData : Resource
{
    [Export] public string Name { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public int MaxAmmoClip { get; set; } = 0;
    [Export] public int Accuracy { get; set; } = 0;
    [Export] public int Range { get; set; } = 0;
    [Export] public int MinDamage { get; set; } = 0;
    [Export] public int MaxDamage { get; set; } = 0;
    [Export] public int Impact { get; set; } = 0;
    [Export] public int CritChance { get; set; } = 0;
    [Export] public PlayerType PlayerType { get; set; }
    [Export] public Texture2D Icon { get; set; }
}