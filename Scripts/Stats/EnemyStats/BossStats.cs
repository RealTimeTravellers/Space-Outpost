using Godot;

public partial class BossStats : StatContainer
{
	[Export] public UnitType UnitType { get; set; } = UnitType.Alien;
	[Export] public EnemyType EnemyType { get; set; } = EnemyType.Boss;


	public BossStats() : base()
	{
		Health = 10;
		Armor = 5;
		Accuracy = 85;
		MovementRange = 5;
		Morale = 40;
		ActionPoints = 3;
		Evasion = 30;
		CriticalHitChance = 50;
		Perception = 95;
	}
}
