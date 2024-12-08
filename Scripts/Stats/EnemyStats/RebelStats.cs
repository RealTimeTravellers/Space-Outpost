using Godot;

public partial class RebelStats : StatContainer
{
    [Export] public EnemyType EnemyType { get; set; } = EnemyType.Rebel;

    public RebelStats() : base()
    {

    }
}