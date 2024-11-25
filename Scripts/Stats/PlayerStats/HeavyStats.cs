using Godot;

public partial class HeavyStats : StatContainer
{
    [Export] public PlayerType PlayerType { get; set; } = PlayerType.Heavy;

    public HeavyStats() : base()
    {
        Health = 8;
        Armor = 5;
        Accuracy = 60;
        MovementRange = 4;
        Morale = 20;
        ActionPoints = 2;
        Evasion = 8;
        CriticalHitChance = 35;
        Perception = 50;
    }
}