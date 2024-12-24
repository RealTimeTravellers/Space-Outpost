using Godot;

public partial class EnemyAIController : Node
{
    public EnemyAIStateMachine _stateMachine {get; private set;}
    private Character _character;
    public bool _isActive = false;

    public override void _Ready()
    {
        base._Ready();
        _character = GetParent<Character>();
        _stateMachine = new EnemyAIStateMachine();
        TurnManager.Instance.TurnChanged += OnTurnChanged;
    }

    public override void _Process(double delta)
    {
        if (_character == null || _character.CompletedTurn)
        {
            return;
        }

        ProcessAI();
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

    private void ProcessAI()
    {
        if (!_isActive) return;

        // AI State Machine güncelleme
        var nextState = _stateMachine.UpdateCurrentState(_character);
        if (nextState != _stateMachine.CurrentState)
        {
            _stateMachine.ChangeState(nextState, _character);
        }

        // Character State Machine'i ProcessPlayerState gibi güncelleyelim
        if (_character.CharacterController._navAgent.IsNavigationFinished())
        {
            CharacterStateType currentState = _character.CharacterController._stateMachine.CurrentStateType;
            _character.CharacterController._stateMachine.UpdateState(_character);
            
            if (_character.CharacterController._stateMachine.CurrentStateType != currentState)
            {
                GD.Print($"Enemy state changed from {currentState} to {_character.CharacterController._stateMachine.CurrentStateType}");
                _character.CharacterController._stateMachine.RequestAnimation(
                    _character.CharacterController._stateMachine.CurrentStateType.ToString().ToLowerInvariant());
            }
        }
    }

    public async void MoveToGrid(GridObject targetGrid)
    {
        if (targetGrid == null) return;
        await _character.Move(targetGrid);
    }

    public void SetState(AIState newState, Character aiCharacter)
    {
        _stateMachine.ChangeState(newState, aiCharacter);
    }


}