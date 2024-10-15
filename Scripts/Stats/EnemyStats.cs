using Godot;

public partial class EnemyStats : UnitStats
{
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Creeper;

    public EnemyStats() : base()
    {
        UnitType = UnitType.Alien;
    }
}