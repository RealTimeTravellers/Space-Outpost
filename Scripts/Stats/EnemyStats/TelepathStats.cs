using Godot;

public partial class TelepathStats : StatContainer
{
    [Export] public UnitType UnitType { get; set; } = UnitType.Alien;
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Telepath;

    public TelepathStats() : base()
    {
        Health = 6;
        Armor = 2;
        Accuracy = 75;
        MovementRange = 5;
        Morale = 40;
        ActionPoints = 2;
        Evasion = 20;
        CriticalHitChance = 30;
        Perception = 85;
    }
}