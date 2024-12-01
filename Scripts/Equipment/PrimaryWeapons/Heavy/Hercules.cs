using Godot;

public partial class Hercules : PrimaryWeapon
{
    public Hercules()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Heavy/HerculesStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Heavy;
        PrimaryWeaponType = PrimaryWeaponType.Hercules;
    }
}