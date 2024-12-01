using Godot;

public partial class Nova : SecondaryWeapon
{
    public Nova()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/SecondaryWeapons/NovaStats.tres");
        Initialize(stats);
        SecondaryWeaponType = SecondaryWeaponType.Nova;
    }
}