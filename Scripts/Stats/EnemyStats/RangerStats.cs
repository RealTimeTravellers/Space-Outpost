using Godot;

public partial class RangerStats : StatContainer
{
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Ranger;

    public RangerStats() : base()
    {

    }
}