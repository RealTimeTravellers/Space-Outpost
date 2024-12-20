using Godot;

public partial class CharacterController : Node
{
    public CharacterStateMachine _stateMachine {get; private set;}
    private Character _character;
    private NavigationAgent3D _navAgent;
    private bool _isActive = false;

    public override void _Ready()
    {
        _character = GetParent<Character>();
        _navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        _stateMachine = new CharacterStateMachine();
        _stateMachine.OnStateChanged += OnStateChanged;
        _stateMachine.OnAnimationRequested += OnAnimationRequested;
        
        TurnManager.Instance.TurnChanged += OnTurnChanged;
        TurnManager.Instance.PlayerMovementChanged += OnPlayerMovementChanged;
    }

    public override void _Process(double delta)
    {
        if (_isActive)
        {
            ProcessPlayerState();
        }
    }

    private void ProcessPlayerState()
    {
        CharacterStateType currentState = _stateMachine.CurrentStateType;
        _stateMachine.UpdateState(_character);
        
        if (_stateMachine.CurrentStateType != currentState)
        {
            GD.Print($"State changed from {currentState} to {_stateMachine.CurrentStateType}");
            _stateMachine.RequestAnimation(_stateMachine.CurrentStateType.ToString().ToLower());
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

    public void SetState(CharacterStateType newState, Character playerCharacter)
    {
        _stateMachine.ChangeState(newState, playerCharacter);
    }

    private void OnStateChanged(CharacterStateType oldState, CharacterStateType newState)
    {
        GD.Print($"Player state changed from {oldState} to {newState} for {_character.Name}");
    }

    public CharacterStateMachine GetStateMachine() => _stateMachine;

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

        }
    }

    private void OnAnimationRequested(string animationName)
    {
        if (_character.AnimatorController != null)
        {
            _character.AnimatorController.PlayAnimation(animationName);
        }
    }
}