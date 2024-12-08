using Godot;

public partial class CreeperStats : StatContainer
{
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Creeper;

    public CreeperStats() : base()
    {

        UnitType = UnitType.Alien;
    }
}