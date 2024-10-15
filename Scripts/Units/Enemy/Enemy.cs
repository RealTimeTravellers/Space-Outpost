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
    public PrimaryWeapon PrimaryWeapon { get; private set; }

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
            EnemyType.Telepath => new TelepathStats(),
            EnemyType.Creeper => new CreeperStats(),
            EnemyType.Seperatist => new MedicStats(),
            EnemyType.Ranger => new RangerStats(),
            EnemyType.Rebel => new RebelStats(),
            EnemyType.Boss => new BossStats(),
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
}
