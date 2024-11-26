using Godot;

public partial class Guardian : PrimaryWeapon
{
	public Guardian()
	{
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Medic/GuardianStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Medic;
        PrimaryWeaponType = PrimaryWeaponType.Guardian;
	}
}
