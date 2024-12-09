using Godot;

public partial class TelepathStats : StatContainer
{
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Telepath;

    public TelepathStats() : base()
    {

    }
}