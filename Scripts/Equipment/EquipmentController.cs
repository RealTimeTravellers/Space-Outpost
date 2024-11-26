using System.Collections.Generic;
using Godot;

public partial class EquipmentController : Node
{
	private PrimaryWeapon _currentPrimaryWeapon;
	private SecondaryWeapon _currentSecondaryWeapon;
	private Accessory _currentAccessory;
	private Weapon _currentWeapon;
	
	private List<PrimaryWeapon> _primaryWeapons = new();
	private List<SecondaryWeapon> _secondaryWeapons = new();
	private List<Accessory> _accessories = new();

	public PrimaryWeapon CurrentPrimaryWeapon => _currentPrimaryWeapon;
	public SecondaryWeapon CurrentSecondaryWeapon => _currentSecondaryWeapon;
	public Accessory CurrentAccessory => _currentAccessory;
	public Weapon CurrentWeapon => _currentWeapon;

	private UnitStats _stats;

	public EquipmentController(UnitStats stats)
	{
		_stats = stats;
	}

	public void EquipPrimaryWeapon(PrimaryWeaponType weaponType)
	{
		var weapon = PrimaryWeaponFactory.CreateWeapon(weaponType);
		
		if (_currentPrimaryWeapon != null)
		{
			_currentPrimaryWeapon.RemoveEffects(_stats);
		}

		_currentPrimaryWeapon = weapon;
		_primaryWeapons.Add(weapon);
		
		if (_currentWeapon == null)
		{
			_currentWeapon = weapon;
			weapon.ApplyEffects(_stats);
		}
	}

	// Belirli bir player type için tüm silahları yükle
	public void LoadWeaponsForPlayerType(PlayerType playerType)
	{
		var weapons = PrimaryWeaponFactory.GetWeaponsForPlayerType(playerType);
		foreach (var weapon in weapons)
		{
			_primaryWeapons.Add(weapon);
		}
		
		if (_primaryWeapons.Count > 0)
		{
			EquipPrimaryWeapon(_primaryWeapons[0].PrimaryWeaponType);
		}
	}

	public void EquipSecondaryWeapon(SecondaryWeaponType weaponType)
	{
		var weapon = SecondaryWeaponFactory.CreateWeapon(weaponType);
		
		if (_currentSecondaryWeapon != null)
		{
			_currentSecondaryWeapon.RemoveEffects(_stats);
		}

		_currentSecondaryWeapon = weapon;
		_secondaryWeapons.Add(weapon);
		
		if (_currentWeapon == null)
		{
			_currentWeapon = weapon;
			weapon.ApplyEffects(_stats);
		}
	}

	public void EquipAccessory(AccessoryType accessoryType)
	{
		var accessory = AccessoryFactory.CreateAccessory(accessoryType);
		
		if (_currentAccessory != null)
		{
			_currentAccessory.RemoveEffects(_stats);
		}

		_currentAccessory = accessory;
		_accessories.Add(accessory);
		accessory.ApplyEffects(_stats);
	}

		public void SwitchWeapon()
	{
		if (_currentWeapon == null) return;

		_currentWeapon.RemoveEffects(_stats);
		
		if (_currentWeapon == _currentPrimaryWeapon)
		{
			_currentWeapon = _currentSecondaryWeapon;
		}
		else
		{
			_currentWeapon = _currentPrimaryWeapon;
		}
		
		_currentWeapon?.ApplyEffects(_stats);
	}

	public bool NeedsReload()
	{
		if (_currentWeapon is PrimaryWeapon primaryWeapon)
		{
			return primaryWeapon.CurrentAmmoClip <= 0;
		}
		if (_currentWeapon is SecondaryWeapon secondaryWeapon)
		{
			return secondaryWeapon.AmmoClip <= 0;
		}
		return false;
	}

	public void Reload()
	{
		// Reload mantığı burada implement edilecek
	}

	public int GetCurrentWeaponDamage()
	{
		return (_currentWeapon as dynamic)?.DealDamage() ?? 0;
	}
	
	public Equipment GetCurrentWeapon()
	{
		return CurrentWeapon;  
	}
}
