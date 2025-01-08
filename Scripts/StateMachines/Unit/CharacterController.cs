using Godot;
using System.Collections.Generic;
public partial class CharacterController : Node
{
    public bool IsEnemyAlerted { get; set; } = false;
    public CharacterStateMachine _stateMachine {get; private set;}
    [Export] public Character _character;
    [Export] public NavigationAgent3D _navAgent {get; private set;}
    [Export] public NavigationRegion3D _navigationRegion;
    private CharacterStateType currentState;
    public Vector3 _finalDestination;
    public Vector3 _currentVelocity;
    public Vector3 _safeVelocity;

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
        _safeVelocity = Vector3.Zero;

        SetState(CharacterStateType.Idle, _character);
    }

    public override void _Process(double delta)
    {
        if (_character.CharacterController._stateMachine.CurrentStateType == CharacterStateType.Death) return;
        ProcessPlayerState();
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (NavigationServer3D.MapGetIterationId(_navAgent.GetNavigationMap()) == 0)
            return;

        if (_stateMachine.CurrentStateType == CharacterStateType.Moving)
        {
            var nextPos = _navAgent.GetNextPathPosition();
            _currentVelocity = _character.GlobalPosition.DirectionTo(nextPos) * _movementSpeed;
            
            if (_navAgent.AvoidanceEnabled)
                _navAgent.SetVelocity(_currentVelocity);
                
            UpdateNavigation();
        }
    }

    public void AdjustNavigation()
    {
        if (_navigationRegion != null)
        {
            var rid = _navigationRegion.GetRid();
            if (rid.IsValid)
            {
                _navAgent.SetNavigationMap(rid);
            }
        }

        // Temel NavigationAgent3D ayarları
        _navAgent.PathDesiredDistance = 0.5f;
        _navAgent.TargetDesiredDistance = 0.5f;
        _navAgent.Radius = 0.4f;
        _navAgent.MaxSpeed = _movementSpeed;
        _navAgent.NeighborDistance = 3.0f;  // Diğer ajanları algılama mesafesi
        _navAgent.MaxNeighbors = 5;
        _navAgent.PathMaxDistance = 3.0f;
        _navAgent.AvoidanceEnabled = true;
    }

    private void UpdateNavigation()
    {
        if (_stateMachine.CurrentStateType == CharacterStateType.Death)
            return;

        var nextPos = _navAgent.GetNextPathPosition();
        if (nextPos != Vector3.Zero)
        {
            // Final hedefe olan mesafeyi kontrol et
            var distanceToFinal = _character.GlobalPosition.DistanceTo(_finalDestination);
            
            if (distanceToFinal > _navAgent.TargetDesiredDistance)
            {
                // Velocity'yi nextPos ve final hedef arasında dengele
                Vector3 toNext = _character.GlobalPosition.DirectionTo(nextPos);
                Vector3 toFinal = _character.GlobalPosition.DirectionTo(_finalDestination);
                Vector3 blendedDirection = (toNext + toFinal * 0.5f).Normalized();
                
                _character.Velocity = blendedDirection * _movementSpeed;
                
                // Karakterin yönünü final hedefe çevir
                var lookAtTarget = new Vector3(_finalDestination.X, _character.GlobalPosition.Y, _finalDestination.Z);
                _character.LookAt(lookAtTarget, Vector3.Up);
                _character.RotateY(Mathf.Pi);
                
                _character.MoveAndSlide();
            }
        }
    }

    private void OnVelocityComputed(Vector3 safeVelocity)
    {
        _safeVelocity = safeVelocity;
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