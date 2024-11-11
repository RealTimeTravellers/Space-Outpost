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
    public PlayerEquipment PlayerEquipment { get; private set; }

    public Player()
    {
        InitializeStats();  
        PlayerEquipment = new PlayerEquipment(Stats);
    }

    protected override void InitializeStats()
    {
        PlayerStats playerStats = new PlayerStats();
        Stats = playerStats.CreateStatsForPlayerType();
        GD.Print("Player stats initialized");
    }

    public void SetInitialEquipment(PrimaryWeapon primaryWeapon, SecondaryWeapon secondaryWeapon, Accessory accessory)
    {
        PlayerEquipment.SetInitialEquipment(primaryWeapon, secondaryWeapon, accessory);
    }

    public void SwitchWeapon()
    {
        PlayerEquipment.SwitchWeapon();
    }

    public bool CanAttack(Unit target)
    {
        return CanInteract(target, PlayerEquipment.CurrentWeapon);
    }
}
