using Godot;

public partial class Thumper : PrimaryWeapon
{
    public Thumper()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Engineer/ThumperStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Engineer;
        PrimaryWeaponType = PrimaryWeaponType.Thumper;
    }
}