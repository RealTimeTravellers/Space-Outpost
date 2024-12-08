using Godot;

[Tool]
public partial class StatContainer : Resource
{
    [Export] public UnitType UnitType { get; set; } = UnitType.Human;
    [Export] public int Health { get; set; } = 10;
    [Export] public int Armor { get; set; } = 10;
    [Export] public int Accuracy { get; set; } = 70;
    [Export] public int MovementRange { get; set; } = 10;
    [Export] public int Morale { get; set; } = 20;
    [Export] public int ActionPoints { get; set; } = 2;
    [Export] public int Perception { get; set; } = 20;
    [Export] public int Evasion { get; set; } = 15;
    [Export] public int CriticalHitChance { get; set; } = 10;
    [Export] public int Cover { get; set; } = 0;
}