using Godot;

public partial class Annihilator : PrimaryWeapon
{
    public Annihilator()
    {
		var stats = ResourceLoader.Load<WeaponStatsContainer>("res://Scripts/Equipment/PrimaryWeapons/Heavy/AnnihilatorStats.tres");
        Initialize(stats);
        PlayerType = PlayerType.Heavy;
        PrimaryWeaponType = PrimaryWeaponType.Annihilator;
    }
}
