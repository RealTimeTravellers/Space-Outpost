using Godot;

public partial class Falcon : SecondaryWeapon
{
    public Falcon()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/SecondaryWeapons/FalconStats.tres");
        Initialize(stats);
        SecondaryWeaponType = SecondaryWeaponType.Falcon;
    }
}