using Godot;

public enum PlayerType
{
    Soldier,
    Sniper,
    Engineer,
    Medic,
    Heavy,
}
public partial class Player : Unit
{
    [Export]
    public PlayerType PlayerType { get; set; }
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

    public Player()
    {
        InitializeStats();  
    }

    protected override void InitializeStats()
    {
        Stats = CreateStatsForPlayerType(PlayerType);
        GD.Print("Player stats initialized");
    }

    private UnitStats CreateStatsForPlayerType(PlayerType type)
    {
        return type switch
        {
            PlayerType.Soldier => new SoldierStats(),
            PlayerType.Sniper => new SniperStats(),
            PlayerType.Engineer => new EngineerStats(),
            PlayerType.Medic => new MedicStats(),
            PlayerType.Heavy => new HeavyStats(),
            _ => new UnitStats() // Default case
        };
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
    }
}
