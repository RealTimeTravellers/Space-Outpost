using System.Threading.Tasks;
using Godot;
using System.Linq;
using System;
using System.Collections.Generic;

public partial class EnemyAIController : Node
{
    protected readonly Vector3[] directions = new[]
    {
        new Vector3(1, 0, 0),   // Sağ
        new Vector3(-1, 0, 0),  // Sol
        new Vector3(0, 0, 1),   // İleri
        new Vector3(0, 0, -1)   // Geri
    };

    public EnemyAIStateMachine _stateMachine {get; private set;}
    [Export] public Character _character;
    [Export] public bool _isActive = false;
    public bool _isDead = false;
    public bool _isRunningAway = false;
    [Export] public bool _isHandlingState {get; set;} = false;


    public override void _Ready()
    {
        base._Ready();

        _stateMachine = new EnemyAIStateMachine();
        TurnManager.Instance.TurnChangedAsync += OnTurnChangedAsync;

        // SetState(AIState.Patrol, _character);
    }

    public override void _ExitTree()
    {
        TurnManager.Instance.TurnChangedAsync -= OnTurnChangedAsync;
    }

    private async Task OnTurnChangedAsync(bool isPlayerTurn)
    {
        if (!isPlayerTurn && _character != null && !_isDead)
        {
            _isActive = true;
            _character.CompletedTurn = false;
            _isHandlingState = false;
            await _character.ProcessAITurn();
        }
        else
        {
            _isActive = false;
            _isHandlingState = true;
        }
    }

    public async Task DecideNextState()
    {
        await _stateMachine.DecideNextState(_character);
    }

    private void ProcessEnemyState()
    {
        if (_isHandlingState) return;
        
        var newState = _stateMachine.ProcessState(_character);
        if (newState != _stateMachine.CurrentState)
            SetState(newState, _character);
    }

    public Character GetNearestPlayer(Vector3 position)
    {
        return TurnManager.Instance.playerCharacters
            .Where(p => p != null && p.CharacterController._stateMachine.CurrentStateType != CharacterStateType.Death)
            .OrderBy(p => position.DistanceTo(p.GlobalPosition))
            .FirstOrDefault();
    }

    public bool RenewTarget(Character character)
    {
        if (character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death)
            return false;

        character.SearchForEnemies();
        
        // Önce enemiesInLos'u kontrol et
        if (character.enemiesInLos.Count > 0)
        {
            character.Target = character.enemiesInLos.FirstOrDefault();
        }
        else
        {
            character.Target = GetNearestPlayer(character.GlobalPosition);
        }

        // Target null check
        if (character.Target == null || 
            character.Target.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death)
        {
            return false;
        }

        float distanceToTarget = character.GlobalPosition.DistanceTo(character.Target.GlobalPosition);
        bool canShoot = distanceToTarget <= character.Perception;
        
        GD.Print($"[AI Debug] {character.Name} renewed target: {character.Target.Name}, " +
                $"Distance: {distanceToTarget}, Perception: {character.Perception}, Can shoot: {canShoot}");
        return canShoot;
    }

    public async Task MoveToGrid(GridObject targetGrid, int maxDistance = 10)
    {
        if (targetGrid == null || targetGrid.IsOccupied || targetGrid.IsBlocked) return;

        // Total mesafe kontrolü
        float totalDistance = _character.GlobalPosition.DistanceTo(targetGrid.GlobalPosition);
        if (totalDistance > maxDistance)
        {
            var direction = (targetGrid.GlobalPosition - _character.GlobalPosition).Normalized();
            var limitedTargetPos = _character.GlobalPosition + direction * maxDistance;
            var closestGrid = GridManager.Instance.GetClosestGrid(limitedTargetPos);
            
            if (closestGrid == null || closestGrid.IsOccupied || closestGrid.IsBlocked || EnemyManager.Instance.IsGridTargeted(closestGrid))
            {
                targetGrid = GridManager.Instance.FindAlternativeGrid(closestGrid, _character.GlobalPosition);
                if (targetGrid == null) return;
            }
            else
                targetGrid = closestGrid;
        }

        if (targetGrid.HasCover && targetGrid.coverType != CoverType.None)
            EnemyManager.Instance.RegisterCoverOccupation(targetGrid, _character);
        
        EnemyManager.Instance.RegisterTargetGrid(targetGrid);
        await _character.Move(targetGrid);
        EnemyManager.Instance.UnregisterTargetGrid(targetGrid);
    }

    public GridObject FindTacticalCover(Character character)
    {
        
        var coverPoints = EnemyManager.Instance.coverGrids;
        if (coverPoints == null || coverPoints.Count == 0)
        {
            GD.Print("[AI Debug] No cover points found");
            return null;
        }

        var nearestPlayer = GetNearestPlayer(character.GlobalPosition);
        if (nearestPlayer == null)
        {
            GD.Print("[AI Debug] No target player found");
            return null;
        }

        GridObject bestCover = null;
        float bestScore = float.MinValue;

        foreach (var coverPoint in coverPoints)
        {
            if (coverPoint.IsOccupied || coverPoint.IsBlocked || 
                EnemyManager.Instance.OccupiedCovers.ContainsKey(coverPoint))
            {
                continue;
            }

            float distanceToPlayer = coverPoint.GlobalPosition.DistanceTo(nearestPlayer.GlobalPosition);
            if (distanceToPlayer > character.Perception)
            {
                continue;
            }

            // How to choose cover:
            // - Distance to player (far = good)
            // - Distance to self (near = good)
            float distanceToSelf = coverPoint.GlobalPosition.DistanceTo(character.GlobalPosition);
            
            // Normalized distances (0-1)
            float normalizedPlayerDistance = distanceToPlayer / character.Perception;
            float normalizedSelfDistance = distanceToSelf / character.Perception;
            
            // Scoring: Player distance is more important, self distance is less important
            float score = (normalizedPlayerDistance * 2.0f) - (normalizedSelfDistance * 0.5f);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestCover = coverPoint;
                GD.Print($"[AI Debug] Found better cover - Distance to player: {distanceToPlayer}, Distance to self: {distanceToSelf}, Score: {score}");
            }
        }

        if (bestCover != null)
        {
            EnemyManager.Instance.RegisterCoverOccupation(bestCover, character);
        }
        else
        {
            bestCover = character.QueryForCover();
            if (bestCover != null)
                EnemyManager.Instance.RegisterCoverOccupation(bestCover, character);
        }
        return bestCover;
    }

    public async Task EnemyShoot()
    {
        _character.CharacterController.SetState(CharacterStateType.Aiming, _character);
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        
        if (_character.Target != null)
        {
            _character.CharacterController.SetState(CharacterStateType.Shooting, _character);
            await ToSignal(GetTree().CreateTimer(.3f), "timeout");
        }
    }


    public void PrepareForHandlingState()
    {
        if (_isHandlingState) return;
        _isHandlingState = true;

        if (_character.CompletedTurn || _character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death) 
        {
            _isHandlingState = false;
            return;
        }
        
        TurnManager.Instance.StartEnemyMovement(_character);
    }

    public async Task HandleAggression()
    {
        PrepareForHandlingState();

        try
        {
            float distanceToTarget = _character.GlobalPosition.DistanceTo(_character.Target.GlobalPosition);

            if (distanceToTarget > _character.Perception/* .Stats.Perception.GetValue() */)
            {
                var grid = GridManager.Instance.GetGridObjectFromWorldPosition(_character.Target.GlobalPosition);
                if (grid != null)
                {
                    await MoveToGrid(grid, 10);
                    await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                }
            }
            else if (_character.actionPoints > 0)
            {
                await EnemyShoot();
                await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            }
        }
        catch (Exception e)
        {
            GD.Print($"[AI Debug] HandleAggression - Error: {e.Message}");
        }
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }

    /// <summary>
    /// This is the tactical state of the enemy.
    /// </summary>
    /// <returns></returns>
    public async Task HandleTactical()
    {
        PrepareForHandlingState();

        try
        {
            bool canShoot = RenewTarget(_character);

            // Eğer cover'daysa ve ateş edebiliyorsa, hemen ateş et
            if (_character.IsInCover && canShoot && _character.actionPoints > 0)
            {
                await EnemyShoot();
                await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                return;
            }

            // Cover'da değilse veya ateş edemiyorsa cover ara
            if (!_character.IsInCover || _character.CharacterController._stateMachine.CurrentStateType != CharacterStateType.InCover)
            {
                var tacticalCover = FindTacticalCover(_character);
                
                if (tacticalCover != null)
                {
                    await MoveToGrid(tacticalCover, 15);
                    await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

                    // Yeni cover'dan ateş kontrolü
                    if (RenewTarget(_character) && _character.actionPoints > 0)
                    {
                        await EnemyShoot();
                        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                    }
                }
            }
            // Cover'daysa ama ateş edemiyorsa daha iyi cover ara
            else if (!canShoot)
            {
                var currentCover = _character.currentGrid;
                var betterCover = FindTacticalCover(_character);
                
                if (betterCover != null)
                {                  
                    EnemyManager.Instance.UnregisterCoverOccupation(currentCover);
                    await MoveToGrid(betterCover, 15);
                    await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                    
                    if (RenewTarget(_character) && _character.actionPoints > 0)
                    {
                        await ToSignal(GetTree().CreateTimer(.1f), "timeout");
                        await EnemyShoot();
                    }
                }
            }
        }
        catch (Exception e)
        {
            GD.Print($"[AI Debug] HandleTactical - Error for {_character.Name}: {e.Message}");
        }
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }

    public async Task HandleCower()
    {
        PrepareForHandlingState();

        try
        {
            await ToSignal(GetTree().CreateTimer(.1f), "timeout");
        }
        catch (Exception e)
        {
            GD.Print($"[AI Debug] HandleCower - Error: {e.Message}");
        }
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }

    /// <summary>
    /// This is the flee state of the enemy.
    /// </summary>
    /// <returns></returns>
    public async Task HandleFlee()
    {
        PrepareForHandlingState();

        try
        {
            if (_character.enemiesInLos.Count > 0)
            {
                var nearestEnemy = _character.enemiesInLos.OrderBy(e => 
                    _character.GlobalPosition.DistanceTo(e.GlobalPosition)).First();
                    
                // Düşmanın tersi yönde bir grid bul
                var fleeDirection = (_character.GlobalPosition - nearestEnemy.GlobalPosition).Normalized();
                var targetPos = _character.GlobalPosition + fleeDirection * 10;
                var fleeGrid = GridManager.Instance.GetGridObjectFromWorldPosition(targetPos);
                
                if (fleeGrid != null && !fleeGrid.IsOccupied)
                {
                    await MoveToGrid(fleeGrid, 10);
                    await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                }
            }
        }
        catch (Exception e)
        {
            GD.Print($"[AI Debug] HandleFlee - Error: {e.Message}");
        }
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }

    /// <summary>
    /// This is the alert state of the enemy.
    /// </summary>
    /// <returns></returns>
    public async Task HandleAlert()
    {
        PrepareForHandlingState();
        
        try 
        {
            if (EnemyManager.Instance.LastShotGrid != null)
            {
                var shotGrid = EnemyManager.Instance.LastShotGrid;
                var neighborGrids = GridManager.Instance.GetNeighborGrids(shotGrid);
                
                var availableNeighbors = neighborGrids
                    .Where(g => g != null && !g.IsOccupied && !g.IsBlocked && 
                            !EnemyManager.Instance.IsGridTargeted(g)) 
                    .OrderBy(g => g.GlobalPosition.DistanceTo(_character.GlobalPosition))
                    .ToList();

                if (availableNeighbors.Any())
                {
                    var targetGrid = availableNeighbors.First();
                    
                    await MoveToGrid(targetGrid, 8);
                    await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                }
            }
        }
        catch (Exception e)
        {
            GD.Print($"[AI Debug] HandleAlert - Error: {e.Message}");
        }
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }

    public async Task HandlePatrol(Character enemy)
    {   
        PrepareForHandlingState();
        
        try
        {
            var random = new RandomNumberGenerator();
            random.Randomize();
            var direction = directions[random.RandiRange(0, directions.Length - 1)];
            var steps = random.RandiRange(1, 2);
            var targetPos = enemy.GlobalPosition + direction * steps * 2; // 2 birim grid size
            
            var targetGrid = GridManager.Instance.GetGridObjectFromWorldPosition(targetPos);
            
            if (targetGrid != null && !targetGrid.IsOccupied && !targetGrid.IsBlocked)
            {
                enemy.CharacterController.SetState(CharacterStateType.Moving, enemy);
                await enemy.Move(targetGrid);
            }
        }
        catch (Exception e)
        {
            GD.Print($"[AI Debug] HandlePatrol - Error: {e.Message}");
        }
        finally
        {
            _isHandlingState = false;
            enemy.CompleteAction(2);
        }
    }

    public void SetState(AIState newState, Character aiCharacter)
    {
        if(_isHandlingState) return;
        GD.Print($"[AI Debug] {aiCharacter.Name} changing to {newState} state");
        _stateMachine.ChangeState(newState, aiCharacter);
    }

    public void PrepareForDispose()
    {
        _isDead = true;
        _isActive = false;
        _isHandlingState = false;
        TurnManager.Instance.TurnChangedAsync -= OnTurnChangedAsync;
        _character = null;
        _stateMachine = null;
    }
}