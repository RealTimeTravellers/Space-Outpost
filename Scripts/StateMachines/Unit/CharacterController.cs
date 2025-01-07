using Godot;
using System.Collections.Generic;
public partial class CharacterController : Node
{
    public bool IsEnemyAlerted { get; set; } = false;
    public CharacterStateMachine _stateMachine {get; private set;}
    [Export] private Character _character;
    [Export] public NavigationAgent3D _navAgent {get; private set;}
    [Export] public NavigationRegion3D _navigationRegion;
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

        AdjustNavigation();

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

    public void AdjustNavigation()
    {
        if (_navigationRegion != null)
        {
            var rid = _navigationRegion.GetRid();
            if (rid.IsValid)
            {
                _navAgent.SetNavigationMap(rid);
                GD.Print($"Navigation map set for {_character.Name}");
            }
        }

        // NavigationAgent3D ayarları
        _navAgent.PathDesiredDistance = 0.2f;
        _navAgent.AvoidancePriority = 1;
        _navAgent.TargetDesiredDistance = 0.2f;
        _navAgent.PathMaxDistance = 0.3f;
        _navAgent.Radius = 0.4f;
        _navAgent.MaxSpeed = _movementSpeed;
        _navAgent.NeighborDistance = 1.0f;
        _navAgent.AvoidanceEnabled = true;

    }

    private void UpdateNavigation()
    {
        if (_stateMachine.CurrentStateType == CharacterStateType.Death)
            return;

        if (_navAgent.IsNavigationFinished())
        {
            _character.Velocity = Vector3.Zero;
            SetState(CharacterStateType.Idle, _character);
            return;
        }

        var nextPos = _navAgent.GetNextPathPosition();
        if (nextPos != Vector3.Zero)
        {
            var direction = (nextPos - _character.GlobalPosition).Normalized();
            var currentSpeed = IsEnemyAlerted ? _alertSpeed : _movementSpeed;

            var velocity = direction * currentSpeed;
            _character.Velocity = velocity;

            var lookAtTarget = new Vector3(nextPos.X, _character.GlobalPosition.Y, nextPos.Z);
            if (!_character.GlobalPosition.IsEqualApprox(lookAtTarget))
            {
                _character.LookAt(lookAtTarget, Vector3.Up);
                _character.RotateY(Mathf.Pi);
            }

            _character.MoveAndSlide();
        }
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