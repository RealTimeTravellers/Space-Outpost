// Sniper Rifles
using Godot;

public partial class Devastator : PrimaryWeapon
{
    public Devastator()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Sniper/DevastatorStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Sniper;
        PrimaryWeaponType = PrimaryWeaponType.Devastator;
    }
}