using Godot;

public partial class SniperStats : StatContainer
{
    [Export] public PlayerType PlayerType { get; set; } = PlayerType.Sniper;

    public SniperStats() : base()
    {
        Health = 5;
        Armor = 2;
        Accuracy = 85;
        MovementRange = 6;
        Morale = 16;
        ActionPoints = 2;
        Evasion = 15;
        CriticalHitChance = 50;
        Perception = 90;
    }
}