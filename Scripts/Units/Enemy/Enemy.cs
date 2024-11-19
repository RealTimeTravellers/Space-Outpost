// using Godot;



// public partial class Enemy : Unit
// {
//     [Export]
//     public EnemyType EnemyType { get; set; }
//     private EnemyAIController _aiController;
//     public EnemyEquipment EnemyEquipment { get; private set; }

//     public override void _Ready()
//     {
//         _aiController = new EnemyAIController();
//         InitializeStats();
//         EnemyEquipment = new EnemyEquipment(Stats);
//         EquipPrimaryWeapon(EnemyEquipment.GetRandomPrimaryWeapon());
//     }

//     public void SetState(AIState newState)
//     {
//         _aiController.SetState(newState, this);
//     }

//     public override void TakeTurn()
//     {
//         _aiController.UpdateAI(this);
//     }

//     protected override void InitializeStats()
//     {
//         EnemyStats enemyStats = new EnemyStats();
//         Stats = enemyStats.CreateStatsForEnemyType(this.EnemyType);
//         GD.Print("Player stats initialized");
//     }

//     public void EquipPrimaryWeapon(PrimaryWeapon weapon)
//     {
//         EnemyEquipment.SetPrimaryWeapon(weapon);
//     }

//     public virtual bool CanAttack(Unit target)
//     {
//         return CanInteract(target, EnemyEquipment.CurrentWeapon);
//     }
// }
