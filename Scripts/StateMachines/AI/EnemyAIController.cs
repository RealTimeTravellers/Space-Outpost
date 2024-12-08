using Godot;
using System;

public partial class EnemyAIController : Node
{
    public EnemyAIStateMachine _stateMachine {get; private set;}
    private Character _character;
    public bool _isActive = false;

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
        if (!_isActive || _character == null || _character.CompletedTurn)
        {
            _isActive = false;
            return;
        }

        if (_character.Stats.ActionPoints.GetValue() <= 0)
        {
            _character.CompletedTurn = true;
            _isActive = false;
            TurnManager.Instance.EndEnemyMovement();
            return;
        }

        ProcessAI();
    }
    private void ProcessAI()
    {
        if (_character == null || _character.CompletedTurn || _character.Stats.ActionPoints.GetValue() <= 0)
        {
            _isActive = false;
            return;
        }

        var nextState = _stateMachine.UpdateCurrentState(_character);
        if (nextState != _stateMachine.CurrentState)
        {
            SetState(nextState, _character);
        }
    }

    private void OnTurnChanged(bool isPlayerTurn)
    {
        GD.Print($"[AI] Turn changed - isPlayerTurn: {isPlayerTurn}, Character: {_character?.Name}");
        
        if (!isPlayerTurn && _character != null && !_character.IsFriendly)
        {
            GD.Print($"[AI] Starting turn for {_character.Name}");
            _isActive = true;
            SetState(AIState.Patrol, _character);
        }
        else
        {
            GD.Print($"[AI] Ending turn for {_character?.Name}");
            _isActive = false;
        }
    }

    private void OnEnemyMovementChanged(bool started)
    {
        if (_character == null || _character.IsFriendly)
            return;
        if (started && !_character.CompletedTurn)
        {
            _isActive = true;
        }
    }

    public void SetState(AIState newState, Character aiCharacter)
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
        if (remainingActionPoints <= 0)
        {
            _character.EndTurn();
            _isActive = false;
        }
        else if (!_character.CompletedTurn)
        {
            ProcessAI();
        }
    }
}