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
        Equipment = new PlayerEquipment(Stats);
    }

    protected override void InitializeStats()
    {
        Stats = PlayerStats.CreateStatsForPlayerType(this.PlayerType);
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
}
