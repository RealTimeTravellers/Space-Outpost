using Godot;

public partial class BossStats : StatContainer
{
	[Export] public EnemyType EnemyType { get; set; } = EnemyType.Boss;


	public BossStats() : base()
	{

		UnitType = UnitType.Alien;
	}
}
