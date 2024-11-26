using Godot;

public partial class PlayerAIController : Node
{
    private PlayerStateMachine _stateMachine;
    private Character _character;
    private bool _isActive = false;

    public override void _Ready()
    {
        _character = GetParent<Character>();
        _stateMachine = new PlayerStateMachine();
        _stateMachine.OnStateChanged += OnStateChanged;
        
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
        PlayerStateType currentState = _stateMachine.CurrentStateType;
        _stateMachine.UpdateState(_character);
        
        if (_stateMachine.CurrentStateType != currentState)
        {
            GD.Print($"State changed from {currentState} to {_stateMachine.CurrentStateType}");
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

    public void SetState(PlayerStateType newState, Character playerCharacter)
    {
        _stateMachine.ChangeState(newState, playerCharacter);
    }

    private void OnStateChanged(PlayerStateType oldState, PlayerStateType newState)
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
        }
    }
}