using Godot;

public partial class CharacterAnimatorController : Node
{
    private AnimationPlayer _animationPlayer;
    private AnimationTree _animationTree;
    private Character _character;
    private CharacterController _characterController;
    private CharacterStateMachine _stateMachine;
    
    // Animation names
    [Export] public string IdleAnimation { get; set; } = "idle";
    [Export] public string ShootingAnimation { get; set; } = "shoot";
    [Export] public string MovingAnimation { get; set; } = "move";
    [Export] public string InCoverAnimation { get; set; } = "in_cover";
    [Export] public string AimingAnimation { get; set; } = "aim";
    [Export] public string DeathAnimation { get; set; } = "death";

    // Blend parameters
    [Export] public float TransitionDuration { get; set; } = 0.25f;
    [Export] public bool UseBlending { get; set; } = true;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationTree = GetNode<AnimationTree>("AnimationTree");
        _character = GetParent<Character>();
        _characterController = _character.GetNode<CharacterController>("CharacterController");
        _stateMachine = _characterController._stateMachine;
        
        if (_character != null && _stateMachine != null)
        {
            _stateMachine.OnStateChanged += HandleStateChanged;
        }
    }

    private void HandleStateChanged(CharacterStateType oldState, CharacterStateType newState)
    {
        string stateName = newState.ToString().ToLower();
        _animationTree.Set("parameters/StateMachine/transition", stateName);
    }

    public void PlayAnimationWithBlend(string animationName, float? customBlendTime = null)
    {
        if (_animationPlayer == null || !_animationPlayer.HasAnimation(animationName))
        {
            GD.PrintErr($"Animation {animationName} not found!");
            return;
        }

        float blendTime = UseBlending ? (customBlendTime ?? TransitionDuration) : 0;
        _animationPlayer.Play(animationName, blendTime);
    }

    public bool IsAnimationPlaying(string animationName)
    {
        return _animationPlayer?.CurrentAnimation == animationName && _animationPlayer.IsPlaying();
    }

    public override void _ExitTree()
    {
        if (_stateMachine != null)
        {
            _stateMachine.OnStateChanged -= HandleStateChanged;
        }
    }
}