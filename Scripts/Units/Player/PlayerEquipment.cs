using System;
using Godot;

public class PlayerEquipment
{
    public PrimaryWeapon PrimaryWeapon { get; private set; }
    public SecondaryWeapon SecondaryWeapon { get; private set; }
    public Accessory Accessory { get; private set; }

    private Equipment _currentWeapon;
    public Equipment CurrentWeapon
    {
        get => _currentWeapon;
        private set
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.RemoveEffects(Stats);
            }
            _currentWeapon = value;
            if (_currentWeapon != null)
            {
                _currentWeapon.ApplyEffects(Stats);
            }
        }
    }

    private UnitStats Stats { get; set; }

    public PlayerEquipment(UnitStats stats)
    {
        Stats = stats ?? throw new ArgumentNullException(nameof(stats), "Stats cannot be null.");
    }

    public void SetInitialEquipment(PrimaryWeapon primaryWeapon, SecondaryWeapon secondaryWeapon, Accessory accessory)
    {
        PrimaryWeapon = primaryWeapon;
        SecondaryWeapon = secondaryWeapon;
        CurrentWeapon = PrimaryWeapon;

        EquipAccessory(accessory);
    }

    public void EquipAccessory(Accessory accessory)
    {
        if (Accessory != null)
        {
            Accessory.RemoveEffects(Stats);
        }

        Accessory = accessory;
        Accessory.ApplyEffects(Stats);
    }

    public void SwitchWeapon()
    {
        CurrentWeapon = CurrentWeapon == PrimaryWeapon ? SecondaryWeapon : PrimaryWeapon;
        GD.Print($"Switched weapon to: {CurrentWeapon.GetType().Name}");
    }
}