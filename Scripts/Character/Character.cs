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
    public bool IsFriendly {get; private set;} = false;

    #region ICombat Variables
    public bool Friendly { get; private set; } // will be set in ready according to subscene preference.
    public int Health { get; private set; }
    public int Damage { get ; private set ; }
    #endregion

    #region ITactical Variables
    public bool TakingCover { get ; private set ; }
    #endregion

    [Export] private int moveCost = 1; // take from a resource data
    [Export] private int firingCost = 2; // take from a resource data
    [Export] private int takeCoverCost = 2; // take from a resource data
    [Export] private int standToEngageCost = 2; // take from a resource data
    [Export] private int supressiveFireCost = 2; // take from a resource data

    private int actionPoints = 2; // take form a resource data
    private bool CompletedTurn = false;

    private Godot.Collections.Array<Character> enemiosInLos = new();
    [Export] private int queriesPerSecond = 10;
    [Export] private bool doQuery = false;

    public override void _Ready()
    {
        InitializeStats();
        TurnManager.Instance.playerCharacters.Add(this);
        TurnManager.Instance.playerCharacterTurns.Add(this, false);
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
        Friendly = IsFriendly;
        if (IsFriendly)
        {
            PlayerStats playerStats = new PlayerStats(); // Initialize stats if player.
            Stats = playerStats.CreateStatsForPlayerType(playerStats.PlayerType);
            Equipment = new PlayerEquipment(Stats);
        }
        else
        {
            EnemyStats enemyStats = new EnemyStats(); // Initialize stats if enemy.
            Stats = enemyStats.CreateStatsForEnemyType(enemyStats.EnemyType);
            Equipment = new EnemyEquipment(Stats);
            stateMachine = new EnemyAIController();
        }
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
        TurnManager.Instance.playerCharacterTurns[this] = true;
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
        actionPoints -= 2;
        // TODO: chance calculations here define if miss or hit
        // if (hit)
            enemy.TakeDamage(Damage);
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

    #region ITactical Implementations
    public void Move(GridObject targetGrid)
    {
        if(!CompletedTurn)
        {
            // do movement
            GlobalPosition = GridManager.Instance.selectedGrid.GlobalPosition; // TEST
            // TODO: make the mesh agent move using characterBody, or tweens if no verticality
            CompleteAction(moveCost);
        }

        throw new NotImplementedException();
    }

    public void TakeCover()
    {
        TakingCover = true;
        CompleteAction(takeCoverCost);
    }

    public void StandToEngage()
    {
        // TODO: apply query and engage protocol
        CompleteAction(standToEngageCost);
        throw new NotImplementedException();
    }

    public void SupressiveFire()
    {
        // TODO: apply supressive fire protocol
        // TODO: empty guns magazine
        CompleteAction(supressiveFireCost);
        throw new NotImplementedException();
    }
    #endregion

    // events
    private void OnTurnChanged(bool playerTurn)
    {
        if (playerTurn)
            IsMyTurn = false;
        
        CompletedTurn = false;
        TakingCover = false;
        
        actionPoints = default; // predefined can make
        //throw new NotImplementedException();
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
