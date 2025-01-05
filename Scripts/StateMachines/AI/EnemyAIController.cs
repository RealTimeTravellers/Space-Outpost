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

    public bool RenewTarget(Character character)
    {
        if (character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death)
            return false;

        character.SearchForEnemies();
        
        if (character.Target == null || character.Target.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death)
            character.Target = character.enemiesInLos.FirstOrDefault();
        
    
        float distanceToTarget = character.GlobalPosition.DistanceTo(character.Target.GlobalPosition);
        bool canShoot = distanceToTarget <= character.Perception;
        
        GD.Print($"[AI Debug] {character.Name} renewed target: {character.Target.Name}, Can shoot: {canShoot}");
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
            
            if (closestGrid == null || closestGrid.IsOccupied || closestGrid.IsBlocked)
            {
                targetGrid = GridManager.Instance.FindAlternativeGrid(closestGrid, _character.GlobalPosition);
                if (targetGrid == null) return;
            }
            else
                targetGrid = closestGrid;
        }

        await _character.Move(targetGrid);
}

    public GridObject FindTacticalCover(Character character)
    {
        if (character.Target == null)
        {
            GD.Print("[AI Debug] FindTacticalCover - Target is null");
            return null;
        }

        float bestScore = float.MinValue;
        GridObject bestCover = null;
        var coverPoints = EnemyManager.Instance.coverGrids;

        GD.Print($"[AI Debug] FindTacticalCover - Checking {coverPoints.Count} cover points");

        foreach (var coverPoint in coverPoints)
        {
            if (EnemyManager.Instance.OccupiedCovers.ContainsKey(coverPoint) || 
                coverPoint.IsBlocked || coverPoint.IsOccupied)
            {
                GD.Print($"[AI Debug] Cover point skipped - Occupied: {EnemyManager.Instance.OccupiedCovers.ContainsKey(coverPoint)}, Blocked: {coverPoint.IsBlocked}, IsOccupied: {coverPoint.IsOccupied}");
                continue;
            }

            float distanceToTarget = coverPoint.GlobalPosition.DistanceTo(character.Target.GlobalPosition);
            float distanceToSelf = coverPoint.GlobalPosition.DistanceTo(character.GlobalPosition);

            if (distanceToTarget > character.Perception * 1.2f)
            {
                GD.Print($"[AI Debug] Cover point skipped - Too far: {distanceToTarget} > {character.Perception * 1.2f}");
                continue;
            }

            Vector3 coverToTarget = (character.Target.GlobalPosition - coverPoint.GlobalPosition).Normalized();
            float coverAngle = Mathf.Abs(coverToTarget.Dot(coverPoint.CoverNormal));

            float distanceScore = 1.0f - (distanceToTarget / (character.Perception * 1.2f));
            float angleScore = coverAngle;
            float proximityPenalty = 0;

            foreach (var occupiedCover in EnemyManager.Instance.OccupiedCovers)
            {
                float distanceToOther = coverPoint.GlobalPosition.DistanceTo(occupiedCover.Key.GlobalPosition);
                if (distanceToOther < 4f)
                    proximityPenalty += (4f - distanceToOther) * 0.25f;
            }

            float finalScore = (distanceScore + angleScore * 2) - proximityPenalty;
            
            GD.Print($"[AI Debug] Cover point score - Distance: {distanceScore}, Angle: {angleScore}, Penalty: {proximityPenalty}, Final: {finalScore}");

            if (finalScore > bestScore)
            {
                bestScore = finalScore;
                bestCover = coverPoint;
            }
        }

        GD.Print($"[AI Debug] FindTacticalCover - Selected cover with score: {bestScore}");
        return bestCover;
    }

    public async Task EnemyShoot()
    {
        _character.CharacterController.SetState(CharacterStateType.Aiming, _character);
        await ToSignal(GetTree().CreateTimer(.8f), "timeout");
        if (_character.Target != null)
        {
            _character.CharacterController.SetState(CharacterStateType.Shooting, _character);
            await ToSignal(GetTree().CreateTimer(.1f), "timeout");
            _character.CharacterController.SetState(CharacterStateType.Idle, _character);
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
            else if (_character.Perception/* .Stats.ActionPoints.GetValue() */ > 0)
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

    public async Task HandleTactical()
    {
        PrepareForHandlingState();
        GD.Print($"[AI Debug] HandleTactical started for {_character.Name}");
        GD.Print($"[AI Debug] IsInCover: {_character.IsInCover}, ActionPoints: {_character.actionPoints}");

        try
        {
            bool canShoot = RenewTarget(_character);
            
            // Cover'da değilse veya cover state'i yanlış kalmışsa
            if (!_character.IsInCover || _character.CharacterController._stateMachine.CurrentStateType != CharacterStateType.InCover)
            {
                GD.Print($"[AI Debug] {_character.Name} not in cover - Finding tactical cover");
                var tacticalCover = FindTacticalCover(_character);
                
                if (tacticalCover != null)
                {
                    GD.Print($"[AI Debug] Cover found for {_character.Name} - Moving to cover");
                    EnemyManager.Instance.RegisterCoverOccupation(tacticalCover, _character);
                    await MoveToGrid(tacticalCover, 15);
                    
                    _character.IsInCover = true;
                    _character.CharacterController.SetState(CharacterStateType.InCover, _character);
                    
                    // Cover'a vardıktan sonra hedefi yenile ve ateş et
                    if (RenewTarget(_character) && _character.actionPoints > 0)
                    {
                        GD.Print($"[AI Debug] {_character.Name} can shoot after moving to cover");
                        await EnemyShoot();
                    }
                }
            }
            // Cover'daysa ve ateş edemiyorsa
            else if (!canShoot)
            {
                GD.Print($"[AI Debug] {_character.Name} in cover but cannot shoot - Finding better cover");
                var currentCover = _character.currentGrid;
                var betterCover = FindTacticalCover(_character);
                
                if (betterCover != null)
                {
                    _character.IsInCover = false;
                    _character.CharacterController.SetState(CharacterStateType.Moving, _character);
                    
                    EnemyManager.Instance.UnregisterCoverOccupation(currentCover);
                    EnemyManager.Instance.RegisterCoverOccupation(betterCover, _character);
                    await MoveToGrid(betterCover, 15);
                    
                    _character.IsInCover = true;
                    _character.CharacterController.SetState(CharacterStateType.InCover, _character);
                    
                    if (RenewTarget(_character) && _character.actionPoints > 0)
                    {
                        await EnemyShoot();
                    }
                }
            }
            // Cover'daysa ve ateş edebiliyorsa
            else if (_character.actionPoints > 0)
            {
                GD.Print($"[AI Debug] {_character.Name} in cover and can shoot");
                await EnemyShoot();
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
    public async Task HandleAlert()
    {
        PrepareForHandlingState();
        
        try 
        {
            if (EnemyManager.Instance.LastShotGrid != null)
            {
                var targetGrid = EnemyManager.Instance.LastShotGrid;
                
                // Eğer hedef grid meşgulse veya bloklanmışsa, komşu gridlerden birini dene
                if (targetGrid.IsOccupied || targetGrid.IsBlocked)
                {
                    GD.Print($"[AI Debug] {_character.Name} Alert - Target grid occupied, finding alternative");
                    targetGrid = GridManager.Instance.FindAlternativeGrid(targetGrid, _character.GlobalPosition);
                    
                    if (targetGrid == null)
                    {
                        GD.Print($"[AI Debug] {_character.Name} Alert - No alternative grid found");
                        return;
                    }
                }

                // Cover state kontrolü

                await MoveToGrid(targetGrid, 8);
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