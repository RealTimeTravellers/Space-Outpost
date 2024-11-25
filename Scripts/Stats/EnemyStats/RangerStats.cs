using Godot;

public partial class RangerStats : StatContainer
{
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Ranger;

    public RangerStats() : base()
    {
        Health = 5;
        Armor = 2;
        Accuracy = 80;
        MovementRange = 7;
        Morale = 22;
        ActionPoints = 3;
        Evasion = 25;
        CriticalHitChance = 40;
        Perception = 90;
    }
}