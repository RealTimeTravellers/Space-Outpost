using Godot;

public partial class CreeperStats : StatContainer
{
    [Export] public UnitType UnitType { get; set; } = UnitType.Alien;
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Creeper;

    public CreeperStats() : base()
    {
        Health = 8;
        Armor = 4;
        Accuracy = 65;
        MovementRange = 6;
        Morale = 40;
        ActionPoints = 2;
        Evasion = 10;
        CriticalHitChance = 0;
        Perception = 70;
    }
}