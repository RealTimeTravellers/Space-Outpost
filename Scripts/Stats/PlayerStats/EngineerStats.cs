using Godot;

[Tool]
public partial class EngineerStats : StatContainer
{
	[Export] public PlayerType PlayerType { get; set; } = PlayerType.Engineer;

	public EngineerStats() : base()
	{
		/*
		Health = 6;
		Armor = 3;
		Accuracy = 65;
		MovementRange = 6;
		Morale = 18;
		ActionPoints = 2;
		Evasion = 10;
		CriticalHitChance = 30;
		Perception = 75;
		*/
	}
}
