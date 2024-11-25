using System;
using System.Collections.Generic;

public enum AccessoryType
{
    AdrenalineShot,
    Binoculars,
    DroneShield,
    Flashbang,
    FragGrenade,
    IncendiaryGrenade,
    KevlarArmor,
    Medpac
}

public static class AccessoryFactory
{
    public static Accessory CreateAccessory(AccessoryType type)
    {
        return type switch
        {
            AccessoryType.AdrenalineShot => new AdrenalineShot(),
            AccessoryType.Binoculars => new Binoculars(),
            AccessoryType.DroneShield => new DroneShield(),
            AccessoryType.Flashbang => new Flashbang(),
            AccessoryType.FragGrenade => new FragGrenade(),
            AccessoryType.IncendiaryGrenade => new IncendiaryGrenade(),
            AccessoryType.KevlarArmor => new KevlarArmor(),
            AccessoryType.Medpac => new Medpac(),
            _ => throw new ArgumentException($"Unknown accessory type: {type}")
        };
    }

    public static List<Accessory> GetAllAccessories()
    {
        return new List<Accessory>
        {
            CreateAccessory(AccessoryType.AdrenalineShot),
            CreateAccessory(AccessoryType.Binoculars),
            CreateAccessory(AccessoryType.DroneShield),
            CreateAccessory(AccessoryType.Flashbang),
            CreateAccessory(AccessoryType.FragGrenade),
            CreateAccessory(AccessoryType.IncendiaryGrenade),
            CreateAccessory(AccessoryType.KevlarArmor),
            CreateAccessory(AccessoryType.Medpac)
        };
    }
}