using System.Threading.Tasks;
using Godot;
using System.Linq;
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
    private Character _character;
    [Export] public bool _isActive = false;
    [Export] private bool _isBeingDestroyed = false;
    [Export] public bool _turnPlayed {get; set;} = false;
    [Export] public bool _isHandlingState {get; set;} = false;


    public override void _Ready()
    {
        base._Ready();
        _character = GetParent<Character>();
        _stateMachine = new EnemyAIStateMachine();
        TurnManager.Instance.TurnChanged += OnTurnChanged;
        SetState(AIState.Patrol, _character);
    }

    public override void _Process(double delta)
    {
        if (!_isActive || _character.IsDead || _character.CompletedTurn || _isHandlingState || 
            !_character.CharacterController._navAgent.IsNavigationFinished())
            return;
        
        _stateMachine.UpdateCurrentState(_character);
        base._Process(delta);
    }

    private async void OnTurnChanged(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            _isActive = false;
            _turnPlayed = false;
        }
        else
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            _isActive = true;
            _turnPlayed = false;
            
            SetState(_stateMachine.CurrentState, _character);
        }
    }

    public async Task MoveToGrid(GridObject targetGrid, int maxDistance = 10)
    {
        if (targetGrid == null) return;

        var agent = _character.CharacterController._navAgent;
        agent.TargetPosition = targetGrid.GlobalPosition;
        
        await ToSignal(GetTree().CreateTimer(.1f), "timeout");
        
        // Save start position
        Vector3 startPos = _character.GlobalPosition;
        Vector3 currentPos = startPos;
        float totalDistance = 0;
        
        // Move along the path and calculate distance
        while (totalDistance < maxDistance)
        {
            Vector3 nextPos = agent.GetNextPathPosition();
            float stepDistance = currentPos.DistanceTo(nextPos);
            
            if (totalDistance + stepDistance > maxDistance)
            {
                // Calculate remaining distance
                float remaining = maxDistance - totalDistance;
                Vector3 direction = (nextPos - currentPos).Normalized();
                Vector3 limitedPoint = currentPos + direction * remaining;
                
                var nearestGrid = GridManager.Instance.GetGridObjectFromWorldPosition(limitedPoint);
                if (nearestGrid != null && !nearestGrid.IsOccupied && nearestGrid != _character.currentGrid)
                    await _character.Move(nearestGrid);
                break;
            }
            
            currentPos = nextPos;
            totalDistance += stepDistance;
            
            if (agent.IsNavigationFinished())
                break;
        }
    }

    public async Task EnemyShoot()
    {
        _character.CharacterController._stateMachine.ChangeState(CharacterStateType.Aiming, _character);
        await ToSignal(GetTree().CreateTimer(.8f), "timeout");
        if (_character.Target != null)
        {
            _character.CharacterController._stateMachine.ChangeState(CharacterStateType.Shooting, _character);
            await ToSignal(GetTree().CreateTimer(.1f), "timeout");
            _character.CharacterController._stateMachine.ChangeState(CharacterStateType.Idle, _character);
        }
    }

    public async Task HandleAggression()
    {
        if (_character.CompletedTurn) return;
        TurnManager.Instance.StartEnemyMovement(_character);
        _isHandlingState = true;

        try
        {
            float distanceToTarget = _character.GlobalPosition.DistanceTo(_character.Target.GlobalPosition);

            if (distanceToTarget > _character.Stats.Perception.GetValue())
            {
                var grid = GridManager.Instance.GetGridObjectFromWorldPosition(_character.Target.GlobalPosition);
                if (grid != null)
                {
                    _character.CharacterController._stateMachine.ChangeState(CharacterStateType.Moving, _character);
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
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }

    public async Task HandleTactical()
    {
        if (_character.CompletedTurn) return;
        TurnManager.Instance.StartEnemyMovement(_character);
        _isHandlingState = true;

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
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }

    public async Task HandleCower()
    {
        if (_character.CompletedTurn) return;
        TurnManager.Instance.StartEnemyMovement(_character);
        _isHandlingState = true;

        try
        {
            await ToSignal(GetTree().CreateTimer(.1f), "timeout");
        }
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }

    public async Task HandleFlee()
    {
        if (_character.CompletedTurn) return;
        TurnManager.Instance.StartEnemyMovement(_character);
        _isHandlingState = true;

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
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }
    public async Task HandleAlert()
    {
        if (_character.CompletedTurn) return;
        _isHandlingState = true;
        TurnManager.Instance.StartEnemyMovement(_character);
        
        try 
        {
            if (EnemyManager.Instance.LastShotGrid != null)
            {
                await MoveToGrid(EnemyManager.Instance.LastShotGrid, 10);
                await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            }
        }
        finally 
        {
            _isHandlingState = false;
            _character.CompleteAction(2);
        }
    }

    public GridObject HandlePatrol(Character enemy)
    {   
        var random = new RandomNumberGenerator();
        random.Randomize();
        var shuffledDirections = directions.OrderBy(x => random.Randf()).ToArray();
        
        foreach (var dir in shuffledDirections)
        {
            int steps = random.RandiRange(1, 2);
            Vector3 targetPos = enemy.GlobalPosition + dir * steps;
            
            var targetGrid = GridManager.Instance.GetGridObjectFromWorldPosition(targetPos);GD.Print($"[AI Debug] HandlePatrol - Checking position {targetPos}, found grid: {targetGrid}");
            
            if (targetGrid != null && !targetGrid.IsOccupied && !targetGrid.IsBlocked)
                return targetGrid;
        }
        
        return null;
    }

    public void SetState(AIState newState, Character aiCharacter)
    {
        GD.Print($"[AI Debug] {aiCharacter.Name} changing to {newState} state");
        _stateMachine.ChangeState(newState, aiCharacter);
    }

    public void PrepareForDestruction()
    {
        _isBeingDestroyed = true;
        _isActive = false;
        SetProcess(false);
    }
}