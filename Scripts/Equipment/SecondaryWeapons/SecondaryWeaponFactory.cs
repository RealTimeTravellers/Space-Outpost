using System;
using System.Collections.Generic;

public enum SecondaryWeaponType
{
    Enforcer,
    Thunderstrike,
    Viper,
    Falcon,
    NovaPlasma
}

public static class SecondaryWeaponFactory
{
    public static SecondaryWeapon CreateWeapon(SecondaryWeaponType type)
    {
        return type switch
        {
            SecondaryWeaponType.Enforcer => new Enforcer(),
            SecondaryWeaponType.Thunderstrike => new Thunderstrike(),
            SecondaryWeaponType.Viper => new Viper(),
            SecondaryWeaponType.Falcon => new Falcon(),
            SecondaryWeaponType.NovaPlasma => new NovaPlasma(),
            _ => throw new ArgumentException($"Unknown weapon type: {type}")
        };
    }

    public static List<SecondaryWeapon> GetAllSecondaryWeapons()
    {
        return new List<SecondaryWeapon>
        {
            CreateWeapon(SecondaryWeaponType.Enforcer),
            CreateWeapon(SecondaryWeaponType.Thunderstrike),
            CreateWeapon(SecondaryWeaponType.Viper),
            CreateWeapon(SecondaryWeaponType.Falcon),
            CreateWeapon(SecondaryWeaponType.NovaPlasma)
        };
    }
}