using Godot;
using System.Collections.Generic;
public partial class CharacterController : Node
{
    public bool IsEnemyAlerted { get; set; } = false;
    public CharacterStateMachine _stateMachine {get; private set;}
    [Export] private Character _character;
    [Export] public NavigationAgent3D _navAgent {get; private set;}
    private CharacterStateType currentState;
    private bool _isActive = false;
    [Export] public float _alertSpeed = 3.0f;
    [Export] public float _movementSpeed = 5.0f;
    private bool _isMoving = false;
    public override void _Ready()
    {
        base._Ready();
        _stateMachine = new CharacterStateMachine();
        _stateMachine.OnStateChanged += OnStateChanged;

        _character.Velocity = Vector3.Zero;
        SetState(CharacterStateType.Idle, _character);
    }

    public override void _Process(double delta)
    {
        if (_character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death) return;

        ProcessPlayerState();
        if (_stateMachine.CurrentStateType == CharacterStateType.Moving) 
            UpdateNavigation();
           
        base._Process(delta);
    }

    private void UpdateNavigation()
    {
        if (_character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death) return;
        
        if (_navAgent.IsNavigationFinished())
        {
            _character.Velocity = Vector3.Zero;
            return;
        }

        var nextPos = _navAgent.GetNextPathPosition();
        var currentPos = _character.GlobalPosition;
        var direction = (nextPos - currentPos).Normalized();
        
        if (!nextPos.IsEqualApprox(currentPos))
        {
            var lookAtPos = new Vector3(nextPos.X, currentPos.Y, nextPos.Z);
            _character.LookAt(lookAtPos, Vector3.Up);
            _character.RotateY(Mathf.Pi);
        }
        
        float currentSpeed = IsEnemyAlerted ? _alertSpeed : _movementSpeed;
        _character.Velocity = direction * currentSpeed;
        _character.MoveAndSlide();
    }
    private void ProcessPlayerState()
    {
        currentState = _stateMachine.CurrentStateType;
        _stateMachine.UpdateState(_character);
        
        if (_stateMachine.CurrentStateType != currentState)
        {
            string animationName = _stateMachine.CurrentStateType.ToString().ToLowerInvariant();
            _stateMachine.RequestAnimation(animationName);
        }
    }

    public void SetState(CharacterStateType newState, Character character)
    {
        if (!CanChangeState()) return;
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
    }

    public bool CanChangeState()
    {
        if (!_character.IsFriendly 
            && !_character.enemyController._isActive 
            && _character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death)
            return false;
        return true;
    }
}