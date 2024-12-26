using Godot;

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
        if (!isPlayerTurn && _character != null && !_character.IsFriendly)
        {
            _isActive = true;
        }
        else
        {
            _isActive = false;
        }
    }

    public async void MoveToGrid(GridObject targetGrid)
    {
        if (targetGrid == null) return;
        await _character.Move(targetGrid);
    }

    public async void HandleAggression()
    {
        TurnManager.Instance.StartEnemyMovement(_character);
        float distanceToTarget = _character.GlobalPosition.DistanceTo(_character.Target.GlobalPosition);
        GD.Print($"[AI] {_character.Name} distance to target: {distanceToTarget}, Perception: {_character.Stats.Perception.GetValue()}");

        if (distanceToTarget > _character.Stats.Perception.GetValue())
        {
            var grid = GridManager.Instance.GetGridObjectFromWorldPosition(_character.Target.GlobalPosition);
            if (grid != null)
            {
                _character.CharacterController._stateMachine.ChangeState(CharacterStateType.Moving, _character);
                await _character.Move(grid);
                return;
            }
        }
        else if (_character.Stats.ActionPoints.GetValue() > 0)
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