using Godot;

public partial class Enforcer : SecondaryWeapon
{
    public Enforcer()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/SecondaryWeapons/EnforcerStats.tres");
        Initialize(stats);
        SecondaryWeaponType = SecondaryWeaponType.Enforcer;
    }
}