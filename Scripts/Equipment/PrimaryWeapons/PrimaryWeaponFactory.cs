
using System;
using System.Collections.Generic;

public enum PrimaryWeaponType
{
	// Soldier Weapons
	Titan,
	Hawkeye,
	Cerberus,
	
	// Sniper Weapons
	Devastator,
	Vanguard,
	Phalanx,
	
	// Heavy Weapons
	Vulcan,
	Hercules,
	Annihilator,
	
	// Engineer Weapons
	Scorpion,
	Razorback,
	Thumper,
	
	// Medic Weapons
	Swift,
	Guardian,
	Suppressor
}
public static class PrimaryWeaponFactory
{
	public static PrimaryWeapon CreateWeapon(PrimaryWeaponType type)
	{
		return type switch
		{
			// Soldier Weapons
			PrimaryWeaponType.Titan => new Titan(),
			PrimaryWeaponType.Hawkeye => new Hawkeye(),
			PrimaryWeaponType.Cerberus => new Cerebus(),
			
			// Sniper Weapons
			PrimaryWeaponType.Devastator => new Devastator(),
			PrimaryWeaponType.Vanguard => new Vanguard(),
			PrimaryWeaponType.Phalanx => new Phalanx(),
			
			// Heavy Weapons
			PrimaryWeaponType.Vulcan => new Vulcan(),
			PrimaryWeaponType.Hercules => new Hercules(),
			PrimaryWeaponType.Annihilator => new Annihilator(),
			
			// Engineer Weapons
			PrimaryWeaponType.Scorpion => new Scorpion(),
			PrimaryWeaponType.Razorback => new Razorback(),
			PrimaryWeaponType.Thumper => new Thumper(),
			
			// Medic Weapons
			PrimaryWeaponType.Swift => new Swift(),
			PrimaryWeaponType.Guardian => new GuardianLight(),
			PrimaryWeaponType.Suppressor => new Suppressor(),
			
			_ => throw new ArgumentException($"Unknown weapon type: {type}")
		};
	}

	public static List<PrimaryWeapon> GetWeaponsForPlayerType(PlayerType playerType)
	{
		return playerType switch
		{
			PlayerType.Soldier => new List<PrimaryWeapon>
			{
				CreateWeapon(PrimaryWeaponType.Titan),
				CreateWeapon(PrimaryWeaponType.Hawkeye),
				CreateWeapon(PrimaryWeaponType.Cerberus)
			},
			PlayerType.Sniper => new List<PrimaryWeapon>
			{
				CreateWeapon(PrimaryWeaponType.Devastator),
				CreateWeapon(PrimaryWeaponType.Vanguard),
				CreateWeapon(PrimaryWeaponType.Phalanx)
			},
			PlayerType.Heavy => new List<PrimaryWeapon>
			{
				CreateWeapon(PrimaryWeaponType.Vulcan),
				CreateWeapon(PrimaryWeaponType.Hercules),
				CreateWeapon(PrimaryWeaponType.Annihilator)
			},
			PlayerType.Engineer => new List<PrimaryWeapon>
			{
				CreateWeapon(PrimaryWeaponType.Scorpion),
				CreateWeapon(PrimaryWeaponType.Razorback),
				CreateWeapon(PrimaryWeaponType.Thumper)
			},
			PlayerType.Medic => new List<PrimaryWeapon>
			{
				CreateWeapon(PrimaryWeaponType.Swift),
				CreateWeapon(PrimaryWeaponType.Guardian),
				CreateWeapon(PrimaryWeaponType.Suppressor)
			},
			_ => new List<PrimaryWeapon>()
		};
	}
}
