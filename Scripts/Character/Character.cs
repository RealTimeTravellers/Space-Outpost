using Godot;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

public partial class Character : CharacterBody3D, ICombat, ITactical
{
    public GridObject currentGrid = null;

    [Export] public UnitStats Stats;
    [Export] private Node Equipment;
    [Export] private Node stateMachine;
    [Export] public bool move = false; // temp for test only
    [Export] public int firstRange = 10; // test
    [Export] public int secondRange = 10; // test
    [Export] public float range = 25; // test

    // More of an idea, make the non identified chracters show up but black
    // only meaning full if there are civilians in the combat zone
    [Export] public float visualRange = 35;

    public bool IsMyTurn {get; private set;} = false;
    [Export] public bool friendly; // FOR TEST

    #region ICombat Variables
    public bool IsFriendly { get; private set; } // will be set in ready according to subscene preference.
    public int Health { get; private set; }
    public int Damage { get ; private set ; }
    #endregion

    #region ITactical Variables
    public bool IsTakingCover { get ; private set ; }
    #endregion

    [Export] private ActionData actionData = null;

    private int actionPoints = 2; // take form a resource data
    public bool CompletedTurn {get; private set;} = false;

    private Godot.Collections.Array<Character> enemiosInLos = new();
    [Export] private int queriesPerSecond = 10;
    [Export] private bool doQuery = false;

    private RandomNumberGenerator rng = new();

    public override void _Ready()
    {
        InitializeStats();
        SubscribeToEvents();
        base._Ready(); // this signal signifies its completed, keep it at the bottom.
    }

    public override void _Process(double delta)
    {
        if (move) // test
        {
            move = !move;
            Move(GridManager.Instance.selectedGrid);
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
        IsFriendly = friendly; // FOR TEST
        if (IsFriendly)
        {
            PlayerStats playerStats = new PlayerStats(); // Initialize stats if player.
            Stats = playerStats.CreateStatsForPlayerType(playerStats.PlayerType);
            Equipment = new PlayerEquipment(Stats);
            
            TurnManager.Instance.playerCharacters.Add(this);
            //TurnManager.Instance.playerCharacterTurns.Add(this, false);
        }
        else
        {
            EnemyStats enemyStats = new EnemyStats(); // Initialize stats if enemy.
            Stats = enemyStats.CreateStatsForEnemyType(enemyStats.EnemyType);
            Equipment = new EnemyEquipment(Stats);
            stateMachine = new EnemyAIController();

            EnemyManager.Instance.allEnemies.Add(this);
            TurnManager.Instance.enemyCharacters.Add(this);
        }

        actionPoints = actionData.defaultActionPoints;
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

    private void CompleteAction(int cost)
    {
        this.actionPoints -= cost;
        CompletedTurn = CheckTurnEnd();
    }

    private bool CheckTurnEnd()
    {
        if (actionPoints <= 0)
        {
            EndTurn();
            return true;
        }
        else
            return false;
    }

    private void EndTurn()
    {
        CompletedTurn = true;
        //TurnManager.Instance.playerCharacterTurns[this] = true;
    }

    private void Die()
    {
        CompletedTurn = true;
        // play Death animation and sound
        throw new NotImplementedException();
    }

    #region ICombat Implementations
    public Godot.Collections.Array<Character> QueryForEnemies(Godot.Collections.Array<Character> enemies)
    {
        // this needs to be active in enemy turn
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

    /// <summary>
    /// Accuracy is dependent on weapon and range as well as skill
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="accuracy"></param>
    public void Attack(Character enemy, float accuracy)
    {
        actionPoints -= actionData.attackCost;

        // TODO: chance calculations here define if miss or hit
        float chance;
        if (enemy.IsTakingCover)
             chance = accuracy * actionData.coverChanceMultiplier;
        else
            chance = accuracy;

        float hitChance = rng.RandfRange(0f, 1f);

        if (hitChance <= chance)
            enemy.TakeDamage(Damage);
            // and play animation
        // else
            // shoot animation but no hit

        throw new NotImplementedException();

    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
            Die();

    }
    #endregion

    #region ITactical Implementations
    public void Move(GridObject targetGrid)
    {
        if(!CompletedTurn)
        {
            // do movement
            GlobalPosition = GridManager.Instance.selectedGrid.GlobalPosition; // TEST
            // TODO: make the mesh agent move using characterBody, or tweens if no verticality
            TurnManager.Instance.StartPlayerMovement();
            TurnManager.Instance.EndPlayerMovement(); // here for test as this moves instantly currently
            CompleteAction(actionData.moveCost);
        }
    }

    public void TakeCover()
    {
        IsTakingCover = true;
        CompleteAction(actionData.takeCoverCost);
    }

    public void StandToEngage()
    {
        // TODO: apply query and engage protocol
        CompleteAction(actionData.standToEngageCost);
        throw new NotImplementedException();
    }

    public void SupressiveFire()
    {
        // TODO: apply supressive fire protocol
        // TODO: empty guns magazine
        CompleteAction(actionData.supressiveFireCost);
        throw new NotImplementedException();
    }
    #endregion

    // events
    private void OnTurnChanged(bool playerTurn)
    {
        if (playerTurn)
        {
            IsMyTurn = false;
            CompletedTurn = false;
            IsTakingCover = false;
            actionPoints = actionData.defaultActionPoints;
        }
    }

    private void OnPlayerMovementChanged(bool started)
    {
        if (!started) // ended
            SearchForEnemies(true); // force a Query once, to detect enemies
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
            SearchForEnemies(true); // to see if this can see enemy
        }
    }
}
