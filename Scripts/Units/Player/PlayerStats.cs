using Godot;

public partial class PlayerStats : UnitStats
{
    [Export] public PlayerType PlayerType { get; set; } = PlayerType.Soldier;

    public PlayerStats() : base()
    {
        UnitType = UnitType.Human;
    }

    public UnitStats CreateStatsForPlayerType()
    {
        return PlayerType switch
        {
            PlayerType.Soldier => new SoldierStats(),
            PlayerType.Sniper => new SniperStats(),
            PlayerType.Engineer => new EngineerStats(),
            PlayerType.Medic => new MedicStats(),
            PlayerType.Heavy => new HeavyStats(),
            _ => throw new ArgumentException("Invalid PlayerType", nameof(type))
        };
    }
}