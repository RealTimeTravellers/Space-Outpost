using Godot;

public partial class Thunderstrike : SecondaryWeapon
{
    public Thunderstrike()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/SecondaryWeapons/ThunderstrikeStats.tres");
        Initialize(stats);
        SecondaryWeaponType = SecondaryWeaponType.Thunderstrike;
    }
}