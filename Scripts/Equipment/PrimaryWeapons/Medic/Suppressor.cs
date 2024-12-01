using Godot;

public partial class Suppressor : PrimaryWeapon
{
    public Suppressor()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Medic/SuppressorStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Medic;
        PrimaryWeaponType = PrimaryWeaponType.Suppressor;
    }
}