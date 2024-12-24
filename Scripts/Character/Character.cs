using Godot;
using System;
using System.Linq;
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
    [Export]
    public PrimaryWeaponType WeaponType { get; private set; }
 	public EquipmentController Equipment { get; private set; }


    // Animator controller
	[Export] public CharacterAnimatorController AnimatorController { get; private set; }
	[Export] public CharacterController CharacterController { get; private set; }
	[Export] public EnemyAIController enemyController { get; private set; }

	[Export] public bool move = false; // temp for test only
	public int FirstMovementRange => Stats.MovementRange.GetValue();
	public int SecondMovementRange => Stats.MovementRange.GetValue();
	[Export] public float range = 25; // test

	// More of an idea, make the non identified chracters show up but black
	// only meaning full if there are civilians in the combat zone
	[Export] public float VisualRange { get; private set; } = 35; 
	public bool IsInCover { get; set; } = false;
	public event Action<int> ActionCompleted;

	#region ICombat Variables
	[Export] public bool IsFriendly { get; private set; } // will be set in ready according to subscene preference.
	public int Health { get; private set; }
	public int Damage { get; private set; }
	#endregion

	#region ITactical Variables
	public bool IsTakingCover { get ; private set ; }
	public bool IsMoving { get; set; }
	#endregion

	[Export] private ActionData actionData = null;

	[Export] public int actionPoints;  // take form a resource data
	[Export] public bool CompletedTurn {get; set;} = false;

	[Export] public Godot.Collections.Array<Character> enemiesInLos {get; set;} = new();
	[Export] private int queriesPerSecond = 10;
	[Export] private bool doQuery = false;
	[Export] private EndTurnState endTurnState = EndTurnState.None;

	[Export] public Character Target { get; set; } = null;
	[Export] public int targetIndex = 0;
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
        if (CharacterController == null)
        {
			GD.Print($"[Character] {this.Name} CharacterController is not assigned.");
            AddChild(CharacterController);
			CharacterController.SetState(CharacterStateType.Idle, this);
        } 
		
		if (IsFriendly)
		{
			// Player Stats
			StatContainer = PlayerStatsFactory.CreateStatsForPlayerType(PlayerType);
			Stats = new PlayerStats(PlayerType, StatContainer);
			Health = Stats.Health.GetValue();
			// Damage = Equipment.GetCurrentWeaponDamage();

			// Player Equipment
			// Equipment = new EquipmentController(Stats);
			// Equipment.EquipPrimaryWeapon(PrimaryWeaponType.Titan);
			// Equipment.EquipSecondaryWeapon(SecondaryWeaponType.Viper);
			// Equipment.EquipAccessory(AccessoryType.FragGrenade);
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
			if (enemyController == null)
			{
				enemyController = new EnemyAIController();
				enemyController.Name = "EnemyAIController";
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

	public async void SearchForEnemies(bool instantSearch = false)
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
					//Attack(TurnManager.CurrentlyMovingCharacter);
					endTurnState = EndTurnState.None;
					doQuery = false;
				}
			}
			else if (endTurnState == EndTurnState.SupressiveFire)
			{
				if (enemiesInLos.Contains(TurnManager.CurrentlyMovingCharacter))
				{
					//Attack(TurnManager.CurrentlyMovingCharacter);
					//Attack(TurnManager.CurrentlyMovingCharacter);
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
		if (this.actionPoints - cost < 0)
		{
			actionPoints = 0;
			Stats.DepleteActionPoints();
		}
		else
		{
			this.actionPoints -= cost;
		}
		ActionCompleted?.Invoke(this.actionPoints);
		
		CheckTurnEnd();

		if (CompletedTurn && CameraManager.Instance.AimingMode)
			CameraManager.ReturnCameraToTactical();
	}

	private void CheckTurnEnd()
	{
		if (this.actionPoints <= 0 && !CompletedTurn)
		{
			GD.Print($"[Character] {this.Name} out of action points");
			EndTurn();
		}
	}

	public void EndTurn()
	{
		if (CompletedTurn || actionPoints > 0)
		{
			GD.Print($"[Character] {this.Name} cannot end turn yet - CompletedTurn:{CompletedTurn}, AP:{actionPoints}");
			return;
		}
		
		// Hareket devam ediyorsa bekle
		if (!CharacterController._navAgent.IsNavigationFinished())
		{
			GD.Print($"[Character] {this.Name} still moving, cannot end turn");
			return;
		}
		
		GD.Print($"[Character] {this.Name} Ending Turn");
		CompletedTurn = true;

		if (IsFriendly)
		{   
			GD.Print($"[Character] {this.Name} ending player movement");
			TurnManager.Instance.EndPlayerMovement(this);
		}
		else
		{
			GD.Print($"[Character] {this.Name} ending enemy movement");
			TurnManager.Instance.EndEnemyMovement(this);
		}
	}

	public void ToggleAim()
	{
		GD.Print($"[Character] {Name} ToggleAim - Current AimingMode: {CameraManager.Instance.AimingMode}");
		
		if (CameraManager.Instance.AimingMode)
		{
			GD.Print($"[Character] {Name} disabling aim mode");
			CameraManager.Instance.AimingMode = false;
			CameraManager.ReturnCameraToTactical();
		}
		else
		{
			GD.Print($"[Character] {Name} enabling aim mode");
			CameraManager.Instance.AimingMode = true;
			CharacterController.SetState(CharacterStateType.Aiming, this);
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

	/// <summary>
	/// Finds nearest (or not) grid that is unoccupied, if it returns null there is none.
	/// </summary>
	/// <param name="nearest"></param>
	/// <returns></returns>
	public GridObject QueryForCover(bool nearest = true)
	{
		// XXX: check for range as well ? 
		if (nearest)
		{
			Godot.Collections.Array<GridObject> sortedList = new();
			
			// find closest grids and sort them
			foreach (GridObject grid in EnemyManager.Instance.coverGrids)
			{
				if (sortedList.Count < 1)
					sortedList.Add(grid);
				else
				{
					for (int i = 0; i < sortedList.Count; i++)
					{
						if (GlobalPosition.DistanceTo(grid.GlobalPosition) < GlobalPosition.DistanceTo(sortedList[i].GlobalPosition))
						{
							sortedList.Insert(i, grid);
							break;
						}
					}
				}
			}

			foreach (GridObject grid in sortedList)
			{
				if (!grid.IsOccupied)
					return grid;
			}
		}
		else
		{
			// something something wont use...
		}

		return null;
	}

	#region ICombat Implementations
	public Godot.Collections.Array<Character> QueryForEnemies(Godot.Collections.Array<Character> enemies, bool limitedFov = false)
	{
		// this needs to be active in enemy turn
		// this needs to be active last time once moving is done

		Godot.Collections.Array<Character> enemiesWithLos = new();

		foreach (Character enemy in enemies.Select(v => (Character)v))
		{
			float distance = enemy.Position.DistanceTo(this.Position);
			if (distance < Stats.Perception.GetValue()) // is in identification range
			{
				var enemyPos = enemy.GlobalPosition + new Vector3(0, 1f, 0);
				var thisPos = this.GlobalPosition + new Vector3(0, 1f, 0);
				CastHit hit = PhysicsCasts.CastLine(this, thisPos, enemyPos, PhysicsCasts.GetCollisionMask(10), true); // Make enemy 10
				
				if (hit.NonEmpty)
					enemiesWithLos.Add(enemy);
					// GD.Print("vURULDU.");

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
	public async void Attack(Character target)
	{
		// TODO: chance calculations here define if miss or hit - done
		// Calculate hit chance based on attacker's accuracy

		float hitChance = Stats.Accuracy.GetValue() / 100f;
		bool hit = GD.Randf() <= hitChance;

		Vector3 direction = (target.Position - Position).Normalized();
   		shootEffect.ProcessMaterial.Set("direction", direction);
		
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

		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

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
	public async Task Move(GridObject targetGrid)
	{
		if(CompletedTurn || targetGrid == null || Stats.ActionPoints.GetValue() <= 0) 
			return;

		// Grid işlemleri
		if(currentGrid != null)
			currentGrid.ClearOccupied();
		
		if(CharacterController._stateMachine.CurrentStateType == CharacterStateType.InCover)
		{
			IsMoving = true;
			await ToSignal(GetTree().CreateTimer(.8f), "timeout");
		}

		// Hedef pozisyonu ayarla
		CharacterController._navAgent.TargetPosition = targetGrid.GlobalPosition;
		
		// State'i Moving'e geçir
		if (IsFriendly)
		{
			TurnManager.Instance.StartPlayerMovement(this);
			CharacterController.SetState(CharacterStateType.Moving, this);
		}
		else
		{
			TurnManager.Instance.StartEnemyMovement(this);
			CharacterController.SetState(CharacterStateType.Moving, this);
		}

		// Hareketin bitmesini bekle
		while (!CharacterController._navAgent.IsNavigationFinished())
		{
			await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
		}

		// Hedef grid'e yerleş
		GlobalPosition = targetGrid.GlobalPosition;
		currentGrid = targetGrid;
		currentGrid.SetOccupied(this);
		
		// Hareketi tamamla
		CompleteAction(actionData.moveCost);
		IsMoving = false;
		
		if (IsFriendly)
			TurnManager.Instance.EndPlayerMovement(this);
		else
			TurnManager.Instance.EndEnemyMovement(this);
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
            // close ınput
			doQuery = true;
			SearchForEnemies();
		}
		else // enemy finished moving
		{
            // open ınput
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
