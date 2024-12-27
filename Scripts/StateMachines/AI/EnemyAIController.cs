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
        if (!isPlayerTurn && _character != null && !_character.IsDead)
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

    public async Task MoveToGrid(GridObject targetGrid, int maxDistance = 10)
    {
        if (targetGrid == null) return;

        if (_character.currentGrid != null)
            _character.currentGrid.ClearOccupied();

        var directionToTarget = (targetGrid.GlobalPosition - _character.GlobalPosition).Normalized();
        var distanceToTarget = _character.GlobalPosition.DistanceTo(targetGrid.GlobalPosition);
        
        // Determine position
        var actualDistance = Mathf.Min(distanceToTarget, maxDistance);
        var targetPosition = _character.GlobalPosition + directionToTarget * actualDistance;
        
        // What is path ? nav server.
        var navMap = _character.CharacterController._navAgent.GetNavigationMap();
        var pathArray = NavigationServer3D.MapGetPath(
            navMap,
            _character.GlobalPosition,
            targetPosition,
            true
        );

        if (pathArray.Length == 0) return;

        GridObject finalGrid = null;
        foreach (var point in pathArray)
        {
            var grid = GridManager.Instance.GetGridObjectFromWorldPosition(point);
            if (grid != null && !grid.IsOccupied && !grid.IsBlocked)
            {
                finalGrid = grid;
            }
        }

        if (finalGrid != null)
            await _character.Move(finalGrid);
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

        if (_character.CompletedTurn) 
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

            if (distanceToTarget > _character.Stats.Perception.GetValue())
            {
                var grid = GridManager.Instance.GetGridObjectFromWorldPosition(_character.Target.GlobalPosition);
                if (grid != null)
                {
                    _character.CharacterController.SetState(CharacterStateType.Moving, _character);
                    await MoveToGrid(grid, 10);
                    await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                }
            }
            else if (_character.Stats.ActionPoints.GetValue() > 0)
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

        try
        {
            var availableCover = _character.QueryForCover();
            
            // Cover'a gitme ve savaşma mantığı
            if (!_character.IsInCover && availableCover != null)
            {
                await MoveToGrid(availableCover, 10);
                await ToSignal(GetTree().CreateTimer(0.8f), "timeout");

                if (_character.Stats.ActionPoints.GetValue() > 0 || _character.actionPoints > 0)
                {
                    await EnemyShoot();
                    await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                }
            }
            else if (_character.IsInCover)
            {
                if (_character.Stats.ActionPoints.GetValue() > 0)
                {
                    await EnemyShoot();
                    await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
                }
            }
        }
        catch (Exception e)
        {
            GD.Print($"[AI Debug] HandleTactical - Error: {e.Message}");
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
                await MoveToGrid(EnemyManager.Instance.LastShotGrid, 10);
                await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
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
}