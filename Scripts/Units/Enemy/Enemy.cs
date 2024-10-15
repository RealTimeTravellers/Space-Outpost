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
    public EnemyEquipment EnemyEquipment { get; private set; }

    public override void _Ready()
    {
        _aiController = new EnemyAIController();
        InitializeStats();
        EnemyEquipment = new EnemyEquipment(Stats);
        EquipPrimaryWeapon(EnemyEquipment.GetRandomPrimaryWeapon());
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
        Stats = CreateStatsForEnemyType(this.EnemyType);
        GD.Print("Player stats initialized");
    }

    public void EquipPrimaryWeapon(PrimaryWeapon weapon)
    {
        EnemyEquipment.SetPrimaryWeapon(weapon);
    }

    public override bool CanAttack(Unit target)
    {
        return base.CanAttack(target, EnemyEquipment.CurrentWeapon);
    }
}
