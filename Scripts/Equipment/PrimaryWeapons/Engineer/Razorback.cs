using Godot;

public partial class Razorback : PrimaryWeapon
{
	public Razorback()
	{
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Engineer/RazorbackStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Engineer;
        PrimaryWeaponType = PrimaryWeaponType.Razorback;
	}
}
