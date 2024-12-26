using System.Threading.Tasks;
using Godot;
using System.Linq;
public partial class EnemyAIController : Node
{
    public EnemyAIStateMachine _stateMachine {get; private set;}
    private Character _character;
    public bool _isActive = false;
    private bool _isBeingDestroyed = false;

    public override void _Ready()
    {
        base._Ready();
        _character = GetParent<Character>();
        _stateMachine = new EnemyAIStateMachine();
        TurnManager.Instance.TurnChanged += OnTurnChanged;
    }

    public override void _Process(double delta)
    {
        if (!_isActive || _character.CompletedTurn || _character.IsDead)
        {
            return;
        }

        _stateMachine.UpdateCurrentState(_character);
    }

    private void OnTurnChanged(bool isPlayerTurn)
    {
        if (!isPlayerTurn && _character != null && !_character.IsDead)
        {
            _isActive = true;
            _character.CompletedTurn = false;
            _character.Stats.ResetActionPoints();
        }
        else
        {
            _isActive = false;
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
        TurnManager.Instance.StartEnemyMovement(_character);
        float distanceToTarget = _character.GlobalPosition.DistanceTo(_character.Target.GlobalPosition);

        if (distanceToTarget > _character.Stats.Perception.GetValue())
        {
            var grid = GridManager.Instance.GetGridObjectFromWorldPosition(_character.Target.GlobalPosition);
            if (grid != null)
            {
                _character.CharacterController._stateMachine.ChangeState(CharacterStateType.Moving, _character);
                await MoveToGrid(grid, 10);
            }
        }
        else if (_character.Stats.ActionPoints.GetValue() > 0)
        {
            await EnemyShoot();
        }

        if (_character.Stats.ActionPoints.GetValue() <= 0)
        {
            _character.CompletedTurn = true;
            TurnManager.Instance.EndEnemyMovement(_character);
        }
    }

    public async Task HandleTactical()
    {
        TurnManager.Instance.StartEnemyMovement(_character);
              
        // Cover bulma
        var availableCover = _character.QueryForCover();
        
        // Cover'a gitme ve savaşma mantığı
        if (!_character.IsInCover && availableCover != null)
        {
            await MoveToGrid(availableCover, 10);
        }
        else if (_character.IsInCover)
        {
            if (_character.Stats.ActionPoints.GetValue() > 0)
            {
                await EnemyShoot();
            }
        }
        
        if (_character.Stats.ActionPoints.GetValue() <= 0)
        {
            _character.CompletedTurn = true;
            TurnManager.Instance.EndEnemyMovement(_character);
        }
    }

    public void SetState(AIState newState, Character aiCharacter)
    {
        _stateMachine.ChangeState(newState, aiCharacter);
    }

    public void PrepareForDestruction()
    {
        _isBeingDestroyed = true;
        _isActive = false;
        SetProcess(false);
    }
}