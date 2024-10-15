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

    public void EquipPrimaryWeapon(PrimaryWeapon weapon)
    {
        if (PrimaryWeapon != null)
            PrimaryWeapon.RemoveEffects(Stats);
        
        PrimaryWeapon = weapon;
        PrimaryWeapon.ApplyEffects(Stats);
    }

    public void EquipSecondaryWeapon(SecondaryWeapon weapon)
    {
        if (SecondaryWeapon != null)
            SecondaryWeapon.RemoveEffects(Stats);
        
        SecondaryWeapon = weapon;
        SecondaryWeapon.ApplyEffects(Stats);
    }

    public void EquipAccessory(Accessory accessory)
    {
        if (Accessory != null)
            Accessory.RemoveEffects(Stats);
        
        Accessory = accessory;
        Accessory.ApplyEffects(Stats);
    }
}
