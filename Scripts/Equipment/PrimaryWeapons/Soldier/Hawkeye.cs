using Godot;

public partial class Hawkeye : PrimaryWeapon
{
    public Hawkeye()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Soldier/HawkeyeStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Soldier;
        PrimaryWeaponType = PrimaryWeaponType.Hawkeye;
    }
}