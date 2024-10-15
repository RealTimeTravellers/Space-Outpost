using Godot;

public partial class PlayerStats : UnitStats
{
    [Export] public PlayerType PlayerType { get; set; } = PlayerType.Soldier;

    public PlayerStats() : base()
    {
        UnitType = UnitType.Human;
    }
}