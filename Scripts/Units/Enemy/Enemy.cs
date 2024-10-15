using Godot;

public enum EnemyType
{
    Telepath,
    Creeper,
    Seperatist,
    Ranger,
    Rebel,
    Boss,
}

public partial class Enemy : Unit
{
    [Export]
    public EnemyType EnemyType { get; set; }
    private EnemyAIController _aiController;

    public override void _Ready()
    {
        _aiController = new EnemyAIController();
        Stats = new UnitStats();
    }

    public void SetState(AIState newState)
    {
        _aiController.SetState(newState, this);
    }

    public override void TakeTurn()
    {
        _aiController.UpdateAI(this);
    }

    protected override void InitializeStats()
    {
        Stats = CreateStatsForEnemyType(EnemyType);
        GD.Print("Player stats initialized");
    }

    private UnitStats CreateStatsForEnemyType(EnemyType type)
    {
        return type switch
        {
            EnemyType.Telepath => new SoldierStats(),
            EnemyType.Creeper => new EngineerStats(),
            EnemyType.Seperatist => new MedicStats(),
            EnemyType.Ranger => new HeavyStats(),
            EnemyType.Rebel => new HeavyStats(),
            EnemyType.Boss => new HeavyStats(),
            _ => new UnitStats() // Default case
        };
    }
}
