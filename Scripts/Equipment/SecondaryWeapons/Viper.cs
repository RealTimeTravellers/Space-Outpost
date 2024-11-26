using Godot;

public partial class Viper : SecondaryWeapon
{
    public Viper()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/SecondaryWeapons/ViperStats.tres");
        Initialize(stats);
        SecondaryWeaponType = SecondaryWeaponType.Viper;
    }
}