using Godot;

public partial class SeperatistStats : StatContainer
{
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Seperatist;

    public SeperatistStats() : base()
    {
        Health = 7;
        Armor = 3;
        Accuracy = 70;
        MovementRange = 5;
        Morale = 16;
        ActionPoints = 2;
        Evasion = 15;
        CriticalHitChance = 35;
        Perception = 75;
    }
}