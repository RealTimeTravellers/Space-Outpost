using Godot;

public partial class Titan : PrimaryWeapon
{
    public Titan()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Soldier/TitanStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Soldier;
        PrimaryWeaponType = PrimaryWeaponType.Titan;
    }
}