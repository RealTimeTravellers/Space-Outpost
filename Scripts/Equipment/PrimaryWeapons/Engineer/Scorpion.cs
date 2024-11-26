using Godot;

public partial class Scorpion : PrimaryWeapon
{
	public Scorpion()
	{
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Engineer/ScorpionStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Engineer;
        PrimaryWeaponType = PrimaryWeaponType.Scorpion;
	}
}
