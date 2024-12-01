using Godot;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

public partial class Character : CharacterBody3D, ICombat, ITactical
{
	public GridObject currentGrid = null;

	// Stats and equipment - EXPORT etme problemi
	[Export] public PlayerType PlayerType { get; private set; } = PlayerType.Soldier;
	[Export] public EnemyType EnemyType { get; private set; } = EnemyType.Creeper;
	public UnitStats Stats;
	public StatContainer StatContainer;

	// Equipment controller
	[Export] public EquipmentController Equipment { get; private set; }

    // Animator controller
    [Export] public CharacterAnimatorController AnimatorController { get; private set; }

	// Character stuff.
	[Export] private NodePath playerControllerPath;
	[Export] private CharacterController characterController;

	// AI stuff.
	[Export] private NodePath aiControllerPath;
	[Export] private EnemyAIController enemyController;

	[Export] public bool move = false; // temp for test only
	public int FirstMovementRange => Stats.MovementRange.GetValue();
	public int SecondMovementRange => Stats.MovementRange.GetValue();
	[Export] public float range = 25; // test

	// More of an idea, make the non identified chracters show up but black
	// only meaning full if there are civilians in the combat zone
	[Export] public float visualRange { get; private set; } = 35; 

	[Export] public bool IsMyTurn {get; private set;} = false; // For DEBUG USE
	public bool IsInCover { get; private set; } = false;

	#region ICombat Variables
	[Export] public bool IsFriendly { get; private set; } // will be set in ready according to subscene preference.
	public int Health { get; private set; }
	public int Damage { get; private set; }
	#endregion

	#region ITactical Variables
	public bool IsTakingCover { get ; private set ; }
	#endregion

	[Export] private ActionData actionData = null;

	private int actionPoints = 2; // take form a resource data
	[Export] public bool CompletedTurn {get; private set;} = false;

	[Export] private Godot.Collections.Array<Character> enemiesInLos = new();
	[Export] private int queriesPerSecond = 10;
	[Export] private bool doQuery = false;
	[Export] private Character target = null;
	[Export] private int targetIndex = 0;
	[Export] public Node3D ShoulderCamera {get; private set;}

	private RandomNumberGenerator rng = new();

	public override void _Ready()
	{
		InitializeStats();
		SubscribeToEvents();
		base._Ready(); // this signal signifies its completed, keep it at the bottom.
	}

	public override void _ExitTree()
	{
		UnsubscribeFromEvents();
		base._ExitTree();
	}

	private void InitializeStats()
	{
        characterController = GetNodeOrNull<CharacterController>(playerControllerPath);
        if (characterController == null)
        {
            characterController = new CharacterController();
            AddChild(characterController);
        }
        
		if (IsFriendly)
		{
			// Player Stats
			StatContainer = PlayerStatsFactory.CreateStatsForPlayerType(PlayerType);
			Stats = new PlayerStats(PlayerType, StatContainer);
			Health = Stats.Health.GetValue();
			Damage = Equipment.GetCurrentWeaponDamage();

			// Player Equipment
			Equipment = new EquipmentController(Stats);
			Equipment.EquipPrimaryWeapon(PrimaryWeaponType.Titan);
			Equipment.EquipSecondaryWeapon(SecondaryWeaponType.Viper);
			Equipment.EquipAccessory(AccessoryType.FragGrenade);
		}
		else
		{
			// Enemy Stats
			StatContainer = EnemyStatsFactory.CreateStatsForEnemyType(EnemyType);
			Stats = new EnemyStats(EnemyType, StatContainer);
			Health = Stats.Health.GetValue();
			Damage = Equipment.GetCurrentWeaponDamage();

			// Enemy Equipment
			Equipment = new EquipmentController(Stats);
			Equipment.EquipPrimaryWeapon(PrimaryWeaponType.Titan);
			Equipment.EquipSecondaryWeapon(SecondaryWeaponType.Viper);
			Equipment.EquipAccessory(AccessoryType.FragGrenade);

			// Enemy AI Controller
			enemyController = GetNodeOrNull<EnemyAIController>(aiControllerPath);
			if (enemyController == null)
			{
				enemyController = new EnemyAIController();
				AddChild(enemyController);
			}
		}

		SubscribeToEvents();
		if (IsFriendly)
		{
			TurnManager.Instance.playerCharacters.Add(this);
		}
		else
		{
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
			enemiesInLos = QueryForEnemies(EnemyManager.Instance.allEnemies);
			doQuery = false;
			return;
		}

		while (doQuery)
		{
			enemiesInLos = QueryForEnemies(EnemyManager.Instance.allEnemies);
			await Task.Delay(Mathf.CeilToInt(1000f / queriesPerSecond));
		}
	}

	private void CompleteAction(int cost)
	{
		GD.Print(this.Name + " Action Done: " + cost);
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
		GD.Print(this.Name + " Completed Turn!");
		CompletedTurn = true;
		//TurnManager.Instance.playerCharacterTurns[this] = true;
	}

	public void AttackMode()
	{
		// TODO: Add tweening to these
		target = enemiesInLos[targetIndex];
		this.LookAt(target.Position);
		CameraManager.Instance.mainCamera.LookAt(target.Position);
		CameraManager.MoveToShoulder(this);
	}

	public void ChangeTarget(bool toLeft = false)
	{
		if (toLeft)
		{
			targetIndex--;
			if (targetIndex <= 0)
				targetIndex = enemiesInLos.Count-1;
		}
		else
		{
			targetIndex++;
			if (targetIndex >= enemiesInLos.Count)
				targetIndex = 0;
		}

		target = enemiesInLos[targetIndex];
		CameraManager.Instance.mainCamera.LookAt(target.Position);
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
				var enemyPos = enemy.GlobalPosition + new Vector3(0, 1f, 0);
				var thisPos = this.GlobalPosition + new Vector3(0, 1f, 0);
				CastHit hit = PhysicsCasts.CastLine(this, thisPos, enemyPos, PhysicsCasts.GetCollisionMask(10), true); // Make enemy 10
				
				if (hit.NonEmpty)
					enemiesWithLos.Add(enemy);
			}
		}
		return enemiesWithLos;
	}

	/// <summary>
	/// Accuracy is dependent on weapon and range as well as skill
	/// </summary>
	/// <param name="target"></param>
	/// <param name="accuracy"></param>
	public void Attack(Character target, float accuracy)
	{
		// TODO: chance calculations here define if miss or hit - done
		// Calculate hit chance based on attacker's accuracy
		float hitChance = Stats.Accuracy.GetValue() / 100f;
		bool hit = GD.Randf() <= hitChance;
		
		if (hit)
		{
			int damage = Equipment.GetCurrentWeaponDamage();
			target.TakeDamage(damage);
			// and play animation
		}
		else
		{
			// shoot animation but no hit
		}
		throw new NotImplementedException();

	}

	#endregion

	#region ITactical Implementations
	public void Move(GridObject targetGrid)
	{
		if(!CompletedTurn)
		{
			if (IsFriendly)
			{
				 // do movement
				GlobalPosition = GridManager.Instance.selectedGrid.GlobalPosition; // TEST
				// TODO: make the mesh agent move using characterBody, or tweens if no verticality
				TurnManager.Instance.StartPlayerMovement();
				CompleteAction(actionData.moveCost);
				TurnManager.Instance.EndPlayerMovement(); // here for test as this moves instantly currently
			}
			else
			{
				// same thing but moves on its own and enemy event runs
				TurnManager.Instance.StartEnemyMovement();
				CompleteAction(actionData.moveCost);
				TurnManager.Instance.EndEnemyMovement(); // here for test as this moves instantly currently
			}
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
		// TODO: empty guns' magazine
		CompleteAction(actionData.supressiveFireCost);
		throw new NotImplementedException();
	}
	#endregion

	// events
	private void OnTurnChanged(bool playerTurn)
	{
		if (IsFriendly && playerTurn)
		{
			IsMyTurn = false;
			CompletedTurn = false;
			IsTakingCover = false;
			actionPoints = actionData.defaultActionPoints;
		}
		else if(!IsFriendly && !playerTurn)
		{
			IsMyTurn = true;
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
		else // enemy finished moving
		{
			doQuery = false;
			SearchForEnemies(true); // to see if this can see enemy
		}
	}


	public void TakeDamage(int damage)
	{
		Health -= damage;

		if (Health <= 0)
			Die();
	}



}
