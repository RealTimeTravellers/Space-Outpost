using Godot;

public partial class SeperatistStats : StatContainer
{
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Seperatist;

    public SeperatistStats() : base()
    {

    }
}