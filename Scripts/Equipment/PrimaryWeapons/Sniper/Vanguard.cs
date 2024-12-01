using Godot;

public partial class Vanguard : PrimaryWeapon
{
    public Vanguard()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Sniper/VanguardStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Sniper;
        PrimaryWeaponType = PrimaryWeaponType.Vanguard;
    }
}
