using Godot;

public partial class SoldierStats : StatContainer
{
    [Export] public PlayerType PlayerType { get; set; } = PlayerType.Soldier;

    public SoldierStats() : base()
    {
        Health = 7;
        Armor = 3;
        Accuracy = 70;
        MovementRange = 7;
        Morale = 22;
        ActionPoints = 2;
        Evasion = 12;
        CriticalHitChance = 40;
        Perception = 60;
    }
}