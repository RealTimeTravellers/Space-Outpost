using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

public partial class Character : CharacterBody3D, ICombat// don't really know why is this character body
{
    public GridObject currentGrid = null;

    // Stats and equipment
    [Export] public UnitStats Stats;

    // Equipment controller
    [Export] public EquipmentController Equipment { get; private set; }

    // AI stuff.
    [Export] private Node stateMachine;
    [Export] private NodePath aiControllerPath;
    private EnemyAIController aiController;

    [Export] public bool move = false; // temp for test only
    [Export] public int firstRange = 10; // test
    [Export] public int secondRange = 10; // test
    [Export] public float range = 25; // test

    // More of an idea, make the non identified chracters show up but black
    // only meaning full if there are civilians in the combat zone
    [Export] public float visualRange = 35;

    public bool IsMyTurn {get; private set;} = false;
    public bool isFriendly {get; private set;} = false;

    #region ICombat variables
    public bool Friendly { get; private set; } // will be set in ready according to subscene preference.
    public int Health { get; private set; }
    public int Damage { get ; private set ; }
    #endregion

    private Godot.Collections.Array<Character> enemiosInLos = new();
    [Export] private int queriesPerSecond = 10;
    [Export] private bool doQuery = false;

    public override void _Ready()
    {
        // read data here?
        base._Ready();
        InitializeStats();
    }

    public override void _Process(double delta)
    {
        if (move) // test
        {
            move = !move;
            GlobalPosition = GridManager.Instance.selectedGrid.GlobalPosition;
        }

        base._Process(delta);
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
        base._ExitTree();
    }

    private void InitializeStats()
    {
        Friendly = isFriendly;
        if (isFriendly)
        {
            // Player Stats
            PlayerStats playerStats = new PlayerStats(); // Initialize stats if player.
            Stats = playerStats.CreateStatsForPlayerType(playerStats.PlayerType);

            // Player Equipment
            Equipment = new EquipmentController(Stats);
            Equipment.EquipPrimaryWeapon(PrimaryWeaponType.Titan);
            Equipment.EquipSecondaryWeapon(SecondaryWeaponType.Viper);
            Equipment.EquipAccessory(AccessoryType.FragGrenade);
        }
        else
        {
            // Enemy Stats
            EnemyStats enemyStats = new EnemyStats(); // Initialize stats if enemy.
            Stats = enemyStats.CreateStatsForEnemyType(enemyStats.EnemyType);

            // Enemy Equipment
            Equipment = new EquipmentController(Stats);
            Equipment.EquipPrimaryWeapon(PrimaryWeaponType.Titan);
            Equipment.EquipSecondaryWeapon(SecondaryWeaponType.Viper);
            Equipment.EquipAccessory(AccessoryType.FragGrenade);

            // Enemy AI Controller
            stateMachine = new EnemyAIController();
            aiController = GetNodeOrNull<EnemyAIController>(aiControllerPath);
            if (aiController == null)
            {
                aiController = new EnemyAIController();
                AddChild(aiController);
            }
        }

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        TurnManager.Instance.TurnChanged += OnTurnChanged;
        TurnManager.Instance.EnemyMovementChanged += OnEnemyMovementChanged;
        TurnManager.Instance.PlayerMovementChanged += OnPlayerMovementChanged;
    }

    private void UnsubscribeFromEvents()
    {
        TurnManager.Instance.TurnChanged -= OnTurnChanged;
        TurnManager.Instance.EnemyMovementChanged -= OnEnemyMovementChanged;
        TurnManager.Instance.PlayerMovementChanged -= OnPlayerMovementChanged;
    }

    private async void SearchForEnemies(bool instantSearch = false)
    {
        if (instantSearch)
        {
            enemiosInLos = QueryForEnemies(EnemyManager.Instance.allEnemies);
            doQuery = false;
            return;
        }

        while (doQuery)
        {
            enemiosInLos = QueryForEnemies(EnemyManager.Instance.allEnemies);
            await Task.Delay(Mathf.CeilToInt(1000f / queriesPerSecond));
        }
    }

    private void Die()
    {
        throw new NotImplementedException();
    }

    #region ICombat Implementations
    public Godot.Collections.Array<Character> QueryForEnemies(Godot.Collections.Array<Character> enemies)
    {
        // this needs to be actiove in enemy turn
        // this needs to be actiove during this characters movement
        // this needs to be active last time once moving is done

        Godot.Collections.Array<Character> enemiesWithLos = new();

        foreach (Character enemy in enemies.Select(v => (Character)v))
        {
            if (enemy.Position.DistanceTo(this.Position) < range) // is in identification range
            {
                CastHit hit = PhysicsCasts.CastLine(this, enemy.Position, this.Position, PhysicsCasts.GetCollisionMask(10), true); // Make enemy 10
                
                if (hit.NonEmpty)
                    enemiesWithLos.Add(enemy);
            }
        }
        return enemiesWithLos;
    }

    public void Attack(ICombat enemy, float chance)
    {
        // TODO: chance calculations here define if miss or hit
        
        // if (hit)
            int damage = Equipment.GetCurrentWeaponDamage();
            enemy.TakeDamage(damage);
            // and play animation
        // else
            // shoot animation but no hit
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
            Die();

    }
    #endregion

    // events
    private void OnTurnChanged(bool playerTurn)
    {
        if (playerTurn)
            IsMyTurn = false;
        
        throw new NotImplementedException();
    }

    private void OnPlayerMovementChanged(bool started)
    {
        if (!started) // ended
            SearchForEnemies(true); // force a Query once.
    }

    private void OnEnemyMovementChanged(bool started)
    {
        if (started) // enemy started moving
        {
            doQuery = true;
            SearchForEnemies();
        }
        else
        {
            doQuery = false;
            SearchForEnemies(true); // force a query once
        }
    }
}
