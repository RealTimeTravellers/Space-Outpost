using Godot;

public partial class CharacterController : Node
{
    public CharacterStateMachine _stateMachine {get; private set;}
    private Character _character;
    public NavigationAgent3D _navAgent {get; private set;}
    private bool _isActive = false;
    [Export] public float _movementSpeed = 5.0f;
    public override void _Ready()
    {
        _character = GetParent<Character>();
        _navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        _stateMachine = new CharacterStateMachine();
        _stateMachine.OnStateChanged += OnStateChanged;
        
        TurnManager.Instance.TurnChanged += OnTurnChanged;
        TurnManager.Instance.PlayerMovementChanged += OnPlayerMovementChanged;
        TurnManager.Instance.EnemyMovementChanged += OnEnemyMovementChanged;

        _character.Velocity = Vector3.Zero;
        SetState(CharacterStateType.Idle, _character);
    }

    public override void _Process(double delta)
    {
        ProcessPlayerState();
        UpdateNavigation();
    }

    private void UpdateNavigation()
    {
        if (_stateMachine.CurrentStateType != CharacterStateType.Moving)
            return;

        var nextPos = _navAgent.GetNextPathPosition();
        var currentPos = _character.GlobalPosition;
        var direction = (nextPos - currentPos).Normalized();
        
        // Karakteri yönlendir
        if (!nextPos.IsEqualApprox(currentPos))
        {
            var lookAtPos = new Vector3(nextPos.X, currentPos.Y, nextPos.Z);
            _character.LookAt(lookAtPos, Vector3.Up);
            _character.RotateY(Mathf.Pi);
        }
        
        // Hareketi uygula
        _character.Velocity = direction * _movementSpeed;
        _character.MoveAndSlide();
    }

    private void ProcessPlayerState()
    {
        CharacterStateType currentState = _stateMachine.CurrentStateType;
        _stateMachine.UpdateState(_character);
        
        if (_stateMachine.CurrentStateType != currentState)
        {
            GD.Print($"State changed from {currentState} to {_stateMachine.CurrentStateType}");
            string animationName = _stateMachine.CurrentStateType.ToString().ToLowerInvariant();
            _stateMachine.RequestAnimation(animationName);
        }
    }

    private void OnTurnChanged(bool isPlayerTurn)
    {
        if (isPlayerTurn && _character != null && _character.IsFriendly)
        {
            _isActive = true;
            ProcessPlayerState();
        }
        else
        {
            _isActive = false;
        }
    }

    private void OnPlayerMovementChanged(bool started)
    {
        _isActive = started && _character.IsFriendly;
    }

    private void OnEnemyMovementChanged(bool started)
    {
        _isActive = started && !_character.IsFriendly;
    }

    public void SetState(CharacterStateType newState, Character character)
    {
        _stateMachine.ChangeState(newState, character);
    }

    private void OnStateChanged(CharacterStateType oldState, CharacterStateType newState)
    {
        GD.Print($"Player state changed from {oldState} to {newState} for {_character.Name}");
    }

    public override void _ExitTree()
    {
        if (_stateMachine != null)
        {
            _stateMachine.OnStateChanged -= OnStateChanged;
        }
        
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.TurnChanged -= OnTurnChanged;
            TurnManager.Instance.PlayerMovementChanged -= OnPlayerMovementChanged;
            TurnManager.Instance.EnemyMovementChanged -= OnEnemyMovementChanged;
        }
    }
}