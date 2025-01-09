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

	[Export] public UnitType UnitType { get; private set; } = UnitType.Human;
	[Export] public int Evasion { get; set; } = 15;
	[Export] public int Perception { get; private set; } = 20;
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
	public int FirstMovementRange => 8;//Stats.MovementRange.GetValue();
	public int SecondMovementRange => 6;//Stats.MovementRange.GetValue();

	// More of an idea, make the non identified chracters show up but black
	// only meaning full if there are civilians in the combat zone
	[Export] public float VisualRange { get; private set; } = 35; 
	public bool IsInCover { get; set; } = false;
	public bool IsSpecialEnemy { get; set; }
	public event Action<int> ActionCompleted;
	public event Action<int, int> HealthChanged;

	#region ICombat Variables
	[Export] public bool IsFriendly { get; private set; } // will be set in ready according to subscene preference.
	public int Health { get; private set; } = 8;
	public int MaxHealth { get; private set; } = 8;
	public int Damage { get; private set; } = 7;
	#endregion

	#region Visual Variables (shader test)
	public MeshInstance3D _meshInstance;
	public ShaderMaterial _originalMaterial;
	public ShaderMaterial _hologramMaterial;
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

	[Export] public Gun gun;
	[Export] ChracterAudio chracterAudioPlayer;

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
			MaxHealth = Stats.Health.GetValue();
			Health = MaxHealth;
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
			MaxHealth = Health;
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
			}
			//enemyController.SetState(AIState.Patrol, this);
		}

		SubscribeToEvents();
		if (IsFriendly)
		{
			if (!TurnManager.Instance.playerCharacters.Contains(this))
				TurnManager.Instance.playerCharacters.Add(this);
			HealthLabel.Modulate = new Color(0,0,1,1);
		}
		else
		{
			if (!TurnManager.Instance.enemyCharacters.Contains(this))
				TurnManager.Instance.enemyCharacters.Add(this);
			if (!EnemyManager.Instance.allEnemies.Contains(this))
				EnemyManager.Instance.allEnemies.Add(this);
			HealthLabel.Modulate = new Color(1,0,0,1);
		}

		UpdateHealthText();
		actionPoints = 2;//Stats.ActionPoints.GetValue();
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
		// safe
		var targetList = IsFriendly ? 
			new Godot.Collections.Array<Character>(TurnManager.Instance.enemyCharacters.Where(e => e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death)) : 
			new Godot.Collections.Array<Character>(TurnManager.Instance.playerCharacters.Where(e => e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death));

		if (instantSearch)
		{
			enemiesInLos = QueryForEnemies(targetList);
			doQuery = false;
			return;
		}

		while (doQuery)
		{
			enemiesInLos = QueryForEnemies(targetList);

			if (endTurnState == EndTurnState.StandToEngage)
			{
				if (enemiesInLos.Contains(TurnManager.CurrentlyMovingCharacter))
				{
					endTurnState = EndTurnState.None;
					doQuery = false;
				}
			}
			else if (endTurnState == EndTurnState.SupressiveFire)
			{
				if (enemiesInLos.Contains(TurnManager.CurrentlyMovingCharacter))
				{
					endTurnState = EndTurnState.None;
					doQuery = false;
				}
			}

			await Task.Delay(Mathf.CeilToInt(1000f / queriesPerSecond));
		}
	}

	public void CompleteAction(int cost)
	{
		if (actionPoints - cost <= 0)
		{
			actionPoints = 0;
			DepleteActionPoints();
			EndTurn();
		}
		else
		{
			actionPoints -= cost;
		}
		ActionCompleted?.Invoke(actionPoints);
		
		CheckTurnEnd();

		if (CompletedTurn && CameraManager.Instance.AimingMode)
			CameraManager.ReturnCameraToTactical();
	}

	public void ResetActionPoints()
	{
		actionPoints = 2;
		//Stats.ResetActionPoints();
		//actionPoints = Stats.ActionPoints.GetValue();
	}

	public void DepleteActionPoints()
	{
		actionPoints = 0;
		//Stats.DepleteActionPoints();
		//actionPoints = Stats.ActionPoints.GetValue();
	}

	private void CheckTurnEnd()
	{
		if (actionPoints <= 0 && !CompletedTurn)
		{
			EndTurn();
		}
	}

	public void EndTurn()
	{
		// Hareket devam ediyorsa bekle
		// if (CompletedTurn) return;
		
		/*
		if (!IsFriendly && CompletedTurn)
		{
			GD.Print($"[Character] {this.Name} ending enemy movement");
			TurnManager.Instance.EndEnemyMovement(this);
			return;
		}

		while (!CharacterController._navAgent.IsNavigationFinished())
		{
			await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
		}
		*/
		
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
		if (CameraManager.Instance.AimingMode)
		{
			CameraManager.Instance.AimingMode = false;
			CameraManager.ReturnCameraToTactical();
			CharacterController.SetState(CharacterStateType.Idle, this);
		}
		else
		{
			var potentialEnemies = QueryForEnemies(new Godot.Collections.Array<Character>(
				IsFriendly ? 
				TurnManager.Instance.enemyCharacters.Where(e => 
					e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death).ToList() : 
				TurnManager.Instance.playerCharacters.Where(e => 
					e.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death).ToList()
			));

			if (potentialEnemies.Count == 0){
				MissionManager.Instance.AddCharacterLog(MissionManager.Instance.logTexts.CharacterNoEnemiesInSightLog, IsFriendly, this.Name);
				return;
			}
				
			CharacterController.SetState(CharacterStateType.Aiming, this);
			CameraManager.Instance.AimingMode = true;
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
		RotateY(Mathf.Pi);
	}

	public void Die()
	{
		
		// Önce AI'ı devre dışı bırak
		if (!IsFriendly && enemyController != null)
		{
			EnemyManager.Instance.OnEnemyDeath(this);
			enemyController.PrepareForDispose();

			if (IsSpecialEnemy)
            {
                EnemyManager.Instance.OnSpecialEnemyDeath();
            }
		}

		CharacterController._navAgent.ProcessMode = ProcessModeEnum.Disabled;
		
		TurnManager.Instance.CharacterDied?.Invoke(this);
		
		// Grid'den temizle
		if (currentGrid != null)
		{
			currentGrid.ClearOccupied();
			currentGrid = null;
		}

		chracterAudioPlayer?.PlayDeathSound(IsFriendly);

		if (!IsFriendly)
			MissionManager.Instance.RecordEnemyKill();
		else
			MissionManager.Instance.RecordAllyLoss();

		//IsDead = true;
		CompletedTurn = true;
		TurnManager.Instance.RemoveCharacterSafely(this);
	}

	private void UpdateHealthText()
	{
		if (Health <0)
			Health = 0;
		HealthLabel.Text = Health +"/"+ MaxHealth;//Stats.Health.GetValue();
		HealthChanged?.Invoke(Health, 8);
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

			foreach (GridObject grid in sortedList )
			{
				if (!grid.IsOccupied && !EnemyManager.Instance.OccupiedCovers.ContainsKey(grid))
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

		    foreach (Character enemy in enemies.Select(v => (Character)v).Where(e => e != this)) 
		{
			float distance = enemy.Position.DistanceTo(this.Position);
			if (distance < Stats.Perception.GetValue()) // is in identification range
			{
				var enemyPos = enemy.GlobalPosition + new Vector3(0, 1f, 0);
				var thisPos = this.GlobalPosition + new Vector3(0, 1f, 0);

				CastHit wallHit = PhysicsCasts.CastLine(this, thisPos, enemyPos, PhysicsCasts.GetCollisionMask(1), false);
				
				if (!wallHit.NonEmpty)
				{
					uint targetLayer = IsFriendly ? 5u : 4u; 
					CastHit characterHit = PhysicsCasts.CastLine(this, thisPos, enemyPos, PhysicsCasts.GetCollisionMask(targetLayer), true);
					
					if (characterHit.NonEmpty && characterHit.Collider == enemy)
					{
						enemiesWithLos.Add(enemy);
					}
				}

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
	public async Task Attack(Character target)
	{
		// safe
		if (target == null || target.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death || actionPoints <= 0) return;
		// Calculate hit chance based on attacker's accuracy

		float hitChance = Stats.Accuracy.GetValue() - target.Stats.Evasion.GetValue();
		hitChance = Mathf.Clamp(hitChance, 10f, 95f);
		bool hit = GD.Randf() <= hitChance / 100f;

		MissionManager.Instance.RecordShot(hit);

		Vector3 direction = (target.Position - Position).Normalized();
		if (!IsFriendly)
			direction = -direction; // Reverse is needed why ?_
		

		if (hit)
		{
			Damage = 5;
			bool isCritical = GD.Randf() <= Stats.CriticalHitChance.GetValue() / 100f;
			int damage = isCritical ? Damage * 2 : Damage;

			if (isCritical)
			{
				MissionManager.Instance.RecordCriticalHit();
				MissionManager.Instance.AddCharacterLog(MissionManager.Instance.logTexts.CharacterCriticalHitLog, !IsFriendly, Name, target.Name);
			}

			int armorValue = target.Stats.Armor.GetValue();
			int armorReduction = GD.RandRange(1, armorValue);
			int finalDamage = Mathf.Max(1, damage - armorReduction);
			
			target.TakeDamage(finalDamage);
			MissionManager.Instance.AddCharacterLog(MissionManager.Instance.logTexts.CharacterHitLog, !IsFriendly, Name, target.Name, finalDamage);
		}
		else
		{
			MissionManager.Instance.AddCharacterLog(MissionManager.Instance.logTexts.CharacterMissedLog, !IsFriendly, Name, target.Name);
		}

		gun.Fire(hit);

		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");

		CharacterController._stateMachine.RequestAnimation("idle");
		await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
		
		CompleteAction(2);
	}

	public void TakeDamage(int damage)
	{
		Health -= damage;

		if (Health <= 0)
		{
			CharacterController.SetState(CharacterStateType.Death, this);
			UpdateHealthText();
			MissionManager.Instance.AddCharacterLog(MissionManager.Instance.logTexts.CharacterDeathLog, !IsFriendly, Name);
		}
		else
		{
			CharacterController.SetState(CharacterStateType.Hit, this);	
			UpdateHealthText();
		}
	}

	public void Reload()
	{
		if (gun.currentAmmo > gun.data.MagazineCapacity)
			return;

		CharacterController.SetState(CharacterStateType.Reloading, this);
	}

	#endregion

	#region ITactical Implementations
	public async Task Move(GridObject targetGrid)
	{
		if (targetGrid == null || actionPoints <= 0 || CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death) 
			return;

		bool secondMovement = currentGrid.Position.DistanceTo(targetGrid.Position) > this.FirstMovementRange;
		currentGrid?.ClearOccupied();
		
		if (CharacterController._stateMachine.CurrentStateType == CharacterStateType.InCover)
		{
			IsMoving = true;
			await ToSignal(GetTree().CreateTimer(.5f), "timeout");
			
			while (CharacterController._stateMachine.CurrentStateType != CharacterStateType.Idle)
			{
				await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
			}
		}

		IsMoving = true;
		CharacterController._finalDestination = targetGrid.GlobalPosition;
		CharacterController._navAgent.TargetPosition = targetGrid.GlobalPosition;
		
		if (IsFriendly)
			TurnManager.Instance.StartPlayerMovement(this);
		
		CharacterController.SetState(CharacterStateType.Moving, this);

		float timeoutTimer = 0f;
		bool wasNavigationFinished = false;
		
		while (!CharacterController._navAgent.IsNavigationFinished())
		{
			if (IsFriendly && CompletedTurn)
				break;
				
			timeoutTimer += 0.1f;
			
			if (wasNavigationFinished && !CharacterController._navAgent.IsNavigationFinished())
			{
				timeoutTimer = 0f;
			}
			
			if (timeoutTimer >= 6f)
			{
				GlobalPosition = targetGrid.GlobalPosition;
				break;
			}
			
			wasNavigationFinished = CharacterController._navAgent.IsNavigationFinished();
			await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
		}

		GlobalPosition = targetGrid.GlobalPosition;
		wasNavigationFinished = true;
		currentGrid = targetGrid;
		currentGrid.SetOccupied(this);
		
		if (secondMovement)
			CompleteAction(2);
		else 
			CompleteAction(actionData.moveCost);

		IsMoving = false;
	}

	public void TakeCover(bool enterCover = true)
	{
		//CompleteAction(actionData.takeCoverCost);
		IsTakingCover = true;
		if (enterCover)
		{
			Stats.Evasion.AddModifier(15);
			endTurnState = EndTurnState.TakingCover;
		}
		else
		{
			Stats.Evasion.RemoveModifier(15);
			IsTakingCover = false;
			endTurnState = EndTurnState.None;
		}
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
			actionPoints = actionData.defaultActionPoints;
			ResetActionPoints();
			endTurnState = EndTurnState.None;
		}
		else if(!IsFriendly && !playerTurn)
		{
			CompletedTurn = false;
			actionPoints = actionData.defaultActionPoints;
			ResetActionPoints();
			endTurnState = EndTurnState.None;
			// await ProcessAITurn();
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

	public async Task ProcessAITurn()
	{
		if (IsFriendly || CompletedTurn) return;

		if (enemyController == null || !enemyController._isActive)
		{
			CompletedTurn = true;
			return;
		}
		
		var nextState = enemyController._stateMachine._states[enemyController._stateMachine.CurrentState].CheckState(this);
		if (nextState != enemyController._stateMachine.CurrentState)
		{
			enemyController.SetState(nextState, this);
		}
		
		await enemyController._stateMachine._states[enemyController._stateMachine.CurrentState].Decide(this);
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

	public void OnCharacterDied(Character diedCharacter)
	{
		if (enemiesInLos.Contains(diedCharacter))
            enemiesInLos.Remove(diedCharacter);
            
        // Clear target if it was the dead character
        if (Target == diedCharacter)
            Target = null;

		SearchForEnemies(true);
	}
	#endregion

	#region Debug
	public async void ApplyHologramEffect()
    {
		GD.Print("Applying hologram effect");
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

		/*

        _meshInstance = GetNode<MeshInstance3D>("CharacterMesh");
        _originalMaterial = (ShaderMaterial)_meshInstance.GetSurfaceOverrideMaterial(0);
        
        // Hologram shader'ını yükle
        _hologramMaterial = GD.Load<ShaderMaterial>("res://Scripts/Shaders/hologram_shader.gdshader");
        
        // Geçiş animasyonu
        float duration = 1.0f;
        float elapsed = 0.0f;
        
        while (elapsed < duration)
        {
            elapsed += (float)GetProcessDeltaTime();
            float t = elapsed / duration;
            _hologramMaterial.SetShaderParameter("transition", t);
            await ToSignal(GetTree(), "process_frame");
        }
        
        // Orijinal materiale geri dön
        _meshInstance.SetSurfaceOverrideMaterial(0, _originalMaterial);
		*/
    }
	#endregion
}
