using Godot;
using System;

public partial class EnemyAIController : Node
{
    public EnemyAIStateMachine _stateMachine {get; private set;}
    private Character _character;
    private bool _isActive = false;

    public override void _Ready()
    {
        base._Ready();
        _character = GetParent<Character>();
        _character.ActionCompleted += OnActionCompleted;
        _stateMachine = new EnemyAIStateMachine();
        _stateMachine.OnStateChanged += OnStateChanged;
        
        // Turn events'e subscribe ol
        TurnManager.Instance.TurnChanged += OnTurnChanged;
        TurnManager.Instance.EnemyMovementChanged += OnEnemyMovementChanged;
    }

    public override void _Process(double delta)
    {
        if (_isActive)
        {
            ProcessAI();
        }
    }

    private void ProcessAI()
    {
        if (!_isActive || _character == null) return;

        // Her action sonrası state'i tekrar kontrol et
        while (!_character.CompletedTurn)
        {
            AIState currentState = _stateMachine.CurrentState;
            AIState nextState = _stateMachine.UpdateCurrentState(_character);
            
            if (nextState != currentState)
            {
                SetState(nextState, _character);
            }
            
            // Action point kalmadıysa döngüden çık
            if (_character.Stats.ActionPoints.GetValue() <= 0)
                break;
        }
    }

    private void OnTurnChanged(bool isPlayerTurn)
    {
        if (!isPlayerTurn && _character != null && !_character.IsFriendly)
        {
            _isActive = true;
            // AI turuna başla
            ProcessAI();
        }
        else
        {
            _isActive = false;
        }
    }

    private void OnEnemyMovementChanged(bool started)
    {
        _isActive = started && !_character.IsFriendly;
    }

    private void SetState(AIState newState, Character aiCharacter)
    {
        _stateMachine.ChangeState(newState, aiCharacter);
    }

    private void OnStateChanged(AIState oldState, AIState newState)
    {
        GD.Print($"AI State changed from {oldState} to {newState} for {_character.Name}");
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
            TurnManager.Instance.EnemyMovementChanged -= OnEnemyMovementChanged;
        }
    }

    private void OnActionCompleted(int remainingActionPoints)
    {
        if (!_character.CompletedTurn)
        {
            ProcessAI(); // Her action sonrası state'i tekrar değerlendir
        }
    }
}