using Godot;

public partial class Cerberus : PrimaryWeapon
{
    public Cerberus()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Soldier/CerberusStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Soldier;
        PrimaryWeaponType = PrimaryWeaponType.Cerberus;
    }
}