using Godot;

public partial class RebelStats : StatContainer
{
    [Export] public UnitType UnitType { get; set; } = UnitType.Human;
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Rebel;

    public RebelStats() : base()
    {
        Health = 6;
        Armor = 3;
        Accuracy = 70;
        MovementRange = 6;
        Morale = 12;
        ActionPoints = 2;
        Evasion = 20;
        CriticalHitChance = 30;
        Perception = 80;
    }
}