using Godot;

[Tool]
public partial class StatContainer : Resource
{
    [Export] public int Health { get; private set; } = 10;
    [Export] public int Armor { get; private set; } = 10;
    [Export] public int Accuracy { get; private set; } = 70;
    [Export] public int MovementRange { get; private set; } = 10;
    [Export] public int Morale { get; private set; } = 20;
    [Export] public int ActionPoints { get; private set; } = 2;
    [Export] public int Perception { get; private set; } = 20;
    [Export] public int Evasion { get; private set; } = 15;
    [Export] public int CriticalHitChance { get; private set; } = 10;
}