using Godot;

public partial class MedicStats : StatContainer
{
	[Export] public UnitType UnitType { get; set; } = UnitType.Human;
	[Export] public PlayerType PlayerType { get; set; } = PlayerType.Medic;

	public MedicStats() : base()
	{
		/*
		Health = 5;
		Armor = 3;
		Accuracy = 50;
		MovementRange = 6;
		Morale = 18;
		ActionPoints = 2;
		Evasion = 10;
		CriticalHitChance = 20;
		Perception = 70;
		*/
	}
}
