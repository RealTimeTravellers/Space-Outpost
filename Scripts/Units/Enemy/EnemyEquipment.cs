using Godot;
using System;

public partial class EnemyEquipment: Node
{
    public Equipment CurrentWeapon { get; private set; }
    private UnitStats Stats { get; set; }

    public EnemyEquipment(UnitStats stats)
    {
        Stats = stats ?? throw new ArgumentNullException(nameof(stats), "Stats cannot be null.");
    }

    public void SetPrimaryWeapon(PrimaryWeapon weapon)
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.RemoveEffects(Stats);
        }
        CurrentWeapon = weapon;
        CurrentWeapon.ApplyEffects(Stats);
    }

    public PrimaryWeapon GetRandomPrimaryWeapon()
    {
        var random = new Random();
        int weaponType = random.Next(3); // 0, 1, or 2

        return weaponType switch
        {
            0 => GetRandomSoldierWeapon(),
            1 => GetRandomEngineerWeapon(),
            2 => GetRandomMedicWeapon(),
            _ => new Hawkeye() // Default to a soldier weapon
        };
    }

    private PrimaryWeapon GetRandomSoldierWeapon()
    {
        var random = new Random();
        int weaponChoice = random.Next(3);

        return weaponChoice switch
        {
            0 => new Hawkeye(),
            1 => new Titan(),
            2 => new Cerberus(),
            _ => new Hawkeye()
        };
    }

    private PrimaryWeapon GetRandomEngineerWeapon()
    {
        var random = new Random();
        int weaponChoice = random.Next(3);

        return weaponChoice switch
        {
            0 => new Razorback(),
            1 => new Scorpion(),
            2 => new Thumper(),
            _ => new Razorback()
        };
    }

    private PrimaryWeapon GetRandomMedicWeapon()
    {
        var random = new Random();
        int weaponChoice = random.Next(2);

        return weaponChoice switch
        {
            0 => new Guardian(),
            1 => new Swift(),
            2 => new Suppressor(),
            _ => new Guardian()
        };
    }
}