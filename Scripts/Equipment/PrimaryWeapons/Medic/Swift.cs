using Godot;

public partial class Swift : PrimaryWeapon
{
    public Swift()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Medic/SwiftStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Medic;
        PrimaryWeaponType = PrimaryWeaponType.Swift;
    }
}