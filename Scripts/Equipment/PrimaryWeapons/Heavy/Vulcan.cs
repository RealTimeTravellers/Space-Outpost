using Godot;

public partial class Vulcan : PrimaryWeapon
{
    public Vulcan()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Heavy/VulcanStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Heavy;
        PrimaryWeaponType = PrimaryWeaponType.Vulcan;
    }
}