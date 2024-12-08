using Godot;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public partial class Character : CharacterBody3D, ICombat, ITactical
{
	[Export] public GridObject currentGrid = null;

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
	//[Export] private CharacterController characterController;

	// AI stuff.
	[Export] private NodePath aiControllerPath;
	[Export] private EnemyAIController enemyController;

	[Export] public bool move = false; // temp for test only
	public int FirstMovementRange => Stats.MovementRange.GetValue();
	public int SecondMovementRange => Stats.MovementRange.GetValue();
	[Export] public float range = 25; // test

	// More of an idea, make the non identified chracters show up but black
	// only meaning full if there are civilians in the combat zone
	[Export] public float VisualRange { get; private set; } = 35; 
	public bool IsInCover { get; private set; } = false;
	public event Action<int> ActionCompleted;

	#region ICombat Variables
	[Export] public bool IsFriendly { get; private set; } // will be set in ready according to subscene preference.
	public int Health { get; private set; }
	public int Damage { get; private set; }
	#endregion

	#region ITactical Variables
	public bool IsTakingCover { get ; private set ; }
	#endregion

	[Export] private ActionData actionData = null;

	[Export] private int actionPoints; // take form a resource data
	[Export] public bool CompletedTurn {get; set;} = false;

	[Export] private Godot.Collections.Array<Character> enemiesInLos = new();
	[Export] private int queriesPerSecond = 10;
	[Export] private bool doQuery = false;
	[Export] private EndTurnState endTurnState = EndTurnState.None;

	[Export] public Character Target { get; private set; } = null;
	[Export] private int targetIndex = 0;
	[Export] public Node3D ShoulderCamera {get; private set;}

	[Export] private GpuParticles3D shootEffect;
	[Export] private Node3D MuzzlePosition; // play muzzle flash and shoot pos

	[Export] private Label3D HealthLabel;
	[Export] private Sprite3D SelectionSprite;
	

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
        /* characterController = GetNodeOrNull<CharacterController>(playerControllerPath);
        if (characterController == null)
        {
            characterController = new CharacterController();
            AddChild(characterController);
        } */
        
		if (IsFriendly)
		{
			// Player Stats
			StatContainer = PlayerStatsFactory.CreateStatsForPlayerType(PlayerType);
			Stats = new PlayerStats(PlayerType, StatContainer);
			Health = Stats.Health.GetValue();
			/* Damage = Equipment.GetCurrentWeaponDamage();

			// Player Equipment
			Equipment = new EquipmentController(Stats);
			Equipment.EquipPrimaryWeapon(PrimaryWeaponType.Titan);
			Equipment.EquipSecondaryWeapon(SecondaryWeaponType.Viper);
			Equipment.EquipAccessory(AccessoryType.FragGrenade); */
		}
		else
		{
			// Enemy Stats
			StatContainer = EnemyStatsFactory.CreateStatsForEnemyType(EnemyType);
			Stats = new EnemyStats(EnemyType, StatContainer);
			Health = Stats.Health.GetValue();
			/* Damage = Equipment.GetCurrentWeaponDamage();

			// Enemy Equipment
			Equipment = new EquipmentController(Stats);
			Equipment.EquipPrimaryWeapon(PrimaryWeaponType.Titan);
			Equipment.EquipSecondaryWeapon(SecondaryWeaponType.Viper);
			Equipment.EquipAccessory(AccessoryType.FragGrenade); */

			// Enemy AI Controller
			enemyController = GetNodeOrNull<EnemyAIController>(aiControllerPath);
			if (enemyController == null)
			{
				enemyController = new EnemyAIController();
				enemyController.Name = "AIController";
				AddChild(enemyController);
				enemyController.SetState(AIState.Patrol, this);
			}
		}

		SubscribeToEvents();
		if (IsFriendly)
		{
			TurnManager.Instance.playerCharacters.Add(this);
			HealthLabel.Modulate = new Color(0,0,1,1);
		}
		else
		{
			EnemyManager.Instance.allEnemies.Add(this);
			TurnManager.Instance.enemyCharacters.Add(this);
			HealthLabel.Modulate = new Color(1,0,0,1);
		}

		UpdateHealthText();
		actionPoints = Stats.ActionPoints.GetValue();
	}

	private void SubscribeToEvents()
	{
		TurnManager.Instance.TurnChanged += OnTurnChanged;
		TurnManager.Instance.EnemyMovementChanged += OnEnemyMovementChanged;
		TurnManager.Instance.PlayerMovementChanged += OnPlayerMovementChanged;
		TurnManager.Instance.CharacterDied += OnCharacterDied;
		GridManager.Instance.SelectionChanged += OnSelectionChanged;
	}

	private void UnsubscribeFromEvents()
	{
		TurnManager.Instance.TurnChanged -= OnTurnChanged;
		TurnManager.Instance.EnemyMovementChanged -= OnEnemyMovementChanged;
		TurnManager.Instance.PlayerMovementChanged -= OnPlayerMovementChanged;
		TurnManager.Instance.CharacterDied -= OnCharacterDied;
		GridManager.Instance.SelectionChanged -= OnSelectionChanged;
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

			if (endTurnState == EndTurnState.StandToEngage)
			{
				if (enemiesInLos.Contains(TurnManager.CurrentlyMovingCharacter))
				{
					Attack(TurnManager.CurrentlyMovingCharacter);
					endTurnState = EndTurnState.None;
					doQuery = false;
				}
			}
			else if (endTurnState == EndTurnState.SupressiveFire)
			{
				if (enemiesInLos.Contains(TurnManager.CurrentlyMovingCharacter))
				{
					Attack(TurnManager.CurrentlyMovingCharacter);
					endTurnState = EndTurnState.None;
					doQuery = false;
				}
			}

			await Task.Delay(Mathf.CeilToInt(1000f / queriesPerSecond));
		}
	}

	private void CompleteAction(int cost)
	{
		GD.Print($"[Character] {this.Name} Action Done: {cost}");
		this.actionPoints -= cost;
		ActionCompleted?.Invoke(this.actionPoints);
		
		if (this.actionPoints <= 0 && !CompletedTurn)
		{
			GD.Print($"[Character] {this.Name} out of action points");
			EndTurn();
		}

		if (CompletedTurn && CameraManager.Instance.AimingMode)
			CameraManager.ReturnCameraToTactical();
	}

	private bool CheckTurnEnd()
	{
		if (this.actionPoints <= 0 && !CompletedTurn)
		{
			GD.Print($"[Character] {this.Name} out of action points");
			EndTurn();
			return true;
		}
		else
			return false;
	}

	public void EndTurn()
	{
		if (CompletedTurn || actionPoints > 0)
		{
			GD.Print($"[Character] {this.Name} cannot end turn yet - CompletedTurn:{CompletedTurn}, AP:{actionPoints}");
			return;
		}
		
		GD.Print($"[Character] {this.Name} Ending Turn");
		CompletedTurn = true;

		if (IsFriendly)
		{
			GD.Print($"[Character] {this.Name} ending player movement");
			TurnManager.Instance.EndPlayerMovement();
		}
		else
		{
			GD.Print($"[Character] {this.Name} ending enemy movement");
			TurnManager.Instance.EndEnemyMovement();
		}
	}

	public void ToggleAim()
	{
		// TODO: Add tweening to these

		if (CameraManager.Instance.AimingMode)
			CameraManager.ReturnCameraToTactical();
		else
		{
			if (actionPoints > 0 && enemiesInLos.Count > 0)  // Array boş değilse devam et
			{
				targetIndex = Mathf.Clamp(targetIndex, 0, enemiesInLos.Count - 1);  // Index'i sınırla
				Target = enemiesInLos[targetIndex];
				LookAt(Target.Position);
				CameraManager.Instance.MainCameraSet.LookAt(Target.Position);
				CameraManager.MoveToShoulder(this);
			}
		}
	}

	public void ChangeTarget(bool toLeft)
	{
		if (enemiesInLos.Count == 0) return; 
		
		if (toLeft)
		{
			targetIndex--;
			if (targetIndex <= 0)
				targetIndex = enemiesInLos.Count - 1;
		}
		else
		{
			targetIndex++;
			if (targetIndex >= enemiesInLos.Count)
				targetIndex = 0;
		}

		Target = enemiesInLos[targetIndex];
		LookAt(Target.Position);
		CameraManager.MoveToShoulder(this);
		CameraManager.Instance.MainCameraSet.LookAt(Target.Position);
	}

	private void Die()
	{
		CompletedTurn = true;
		// TODO: play Death animation and sound
		
		// Test
		if (IsFriendly)
			TurnManager.Instance.playerCharacters.Remove(this);
		else
		{
			enemiesInLos.Remove(this);
			EnemyManager.Instance.allEnemies.Remove(this);
			TurnManager.Instance.enemyCharacters.Remove(this);
			// need to do a global query check

		}

		TurnManager.Instance.CharacterDied.Invoke(this);
		QueueFree();
	}

	private void UpdateHealthText()
	{
		HealthLabel.Text = Health +"/"+ Stats.Health.GetValue();
	}

	#region ICombat Implementations
	public Godot.Collections.Array<Character> QueryForEnemies(Godot.Collections.Array<Character> enemies, bool limitedFov = false)
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

				if (limitedFov)
				{
					// TODO: do vector math to determine if where character looking direction is within a radian of x degrees
					// TODO; if they are not take them out of enemiesWithLos list
				}
			}
		}
		return enemiesWithLos;
	}

	/// <summary>
	/// Accuracy is dependent on weapon and range as well as skill
	/// </summary>
	/// <param name="target"></param>
	/// <param name="accuracy"></param>
	public void Attack(Character target)
	{
		// TODO: chance calculations here define if miss or hit - done
		// Calculate hit chance based on attacker's accuracy
		float hitChance = Stats.Accuracy.GetValue() / 100f;
		bool hit = GD.Randf() <= hitChance;
		
		if (hit) // || true for test purposes
		{
			shootEffect.ProcessMaterial.Set("spread", 2);
			shootEffect.Restart();

			int damage = 3; //Equipment.GetCurrentWeaponDamage(); // temporary
			target.TakeDamage(damage);
			// and play animation
		}
		else
		{
			shootEffect.ProcessMaterial.Set("spread", 10);
			shootEffect.Restart();

			// shoot animation but no hit
		}

		CompleteAction(actionData.attackCost);
	}

	public void TakeDamage(int damage)
	{
		Health -= damage;

		if (Health <= 0)
			Die();
		else
			UpdateHealthText();
	}

	#endregion

	#region ITactical Implementations
	public void Move(GridObject targetGrid)
	{
		if(CompletedTurn || targetGrid == null || Stats.ActionPoints.GetValue() <= 0) 
			return;

		// Eski grid'i temizle
		if(currentGrid != null)
			currentGrid.ClearOccupied();
			
		// Yeni grid'e taşın
		GlobalPosition = targetGrid.GlobalPosition;
		currentGrid = targetGrid;
		currentGrid.SetOccupied(this);
		
		if (IsFriendly)
		{
			TurnManager.Instance.StartPlayerMovement();
			CompleteAction(actionData.moveCost);
			TurnManager.Instance.EndPlayerMovement();
		}
		else
		{
			TurnManager.Instance.StartEnemyMovement();
			CompleteAction(actionData.moveCost);
			TurnManager.Instance.EndEnemyMovement();
		}
	}

	public void TakeCover()
	{
		CompleteAction(actionData.takeCoverCost);
		IsTakingCover = true;
		endTurnState = EndTurnState.TakingCover;
	}

	public void StandToEngage()
	{
		// TODO: apply query and engage protocol
		CompleteAction(actionData.standToEngageCost);
		endTurnState = EndTurnState.StandToEngage;
	}

	public void SupressiveFire()
	{
		// TODO: apply supressive fire protocol
		// TODO: empty guns' magazine
		CompleteAction(actionData.supressiveFireCost);
		endTurnState = EndTurnState.SupressiveFire;
	}
	#endregion

	#region Event Handles
	private void OnTurnChanged(bool playerTurn)
	{
		if (IsFriendly && playerTurn)
		{
			CompletedTurn = false;
			IsTakingCover = false;
			actionPoints = actionData.defaultActionPoints;
			endTurnState = EndTurnState.None;
		}
		else if(!IsFriendly && !playerTurn)
		{
			CompletedTurn = false;
			IsTakingCover = false;
			actionPoints = actionData.defaultActionPoints;
			endTurnState = EndTurnState.None;
		}
	}

	private void OnSelectionChanged(GridObject gridObject)
	{
		if (IsFriendly && SelectionSprite != null) 
		{
			if (gridObject == null) return;

			if (this == GridManager.Instance.selectedCharacter)
				SelectionSprite.Visible = true;
			else
				SelectionSprite.Visible = false;
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

	private void OnCharacterDied(Character diedCharacter)
	{
		SearchForEnemies(true);
	}
	#endregion
}
