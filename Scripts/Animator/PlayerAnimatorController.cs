using Godot;

public partial class PlayerAnimatorController : Node
{
    private AnimationPlayer _animationPlayer;
    private Character _character;
    private PlayerStateMachine _stateMachine;
    
    // Animation names - can be configured in the editor
    [Export] public string IdleAnimation { get; set; } = "idle";
    [Export] public string ShootingAnimation { get; set; } = "shoot";
    [Export] public string ReloadingAnimation { get; set; } = "reload";
    [Export] public string MovingAnimation { get; set; } = "move";
    [Export] public string TakingCoverAnimation { get; set; } = "take_cover";
    [Export] public string LeavingCoverAnimation { get; set; } = "leave_cover";
    [Export] public string AimingAnimation { get; set; } = "aim";
    [Export] public string DeathAnimation { get; set; } = "death";
    [Export] public string TacticalStanceAnimation { get; set; } = "tactical_stance";

    public override void _Ready()
    {
        _character = GetParent<Character>();
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _stateMachine = GetNode<PlayerStateMachine>("PlayerStateMachine");
        
        if (_stateMachine != null)
        {
            _stateMachine.OnStateChanged += HandleStateChanged;
        }
    }

    private void HandleStateChanged(PlayerStateType oldState, PlayerStateType newState)
    {
        switch (newState)
        {
            case PlayerStateType.Idle:
                PlayAnimation(IdleAnimation);
                break;
                
            case PlayerStateType.Shooting:
                PlayAnimation(ShootingAnimation);
                break;
                
            case PlayerStateType.Reloading:
                PlayAnimation(ReloadingAnimation);
                break;
                
            case PlayerStateType.Moving:
                PlayAnimation(MovingAnimation);
                break;
                
            case PlayerStateType.TakingCover:
                PlayAnimation(TakingCoverAnimation);
                break;
                
            case PlayerStateType.LeavingCover:
                PlayAnimation(LeavingCoverAnimation);
                break;
                
            case PlayerStateType.Aiming:
                PlayAnimation(AimingAnimation);
                break;
                
            case PlayerStateType.Death:
                PlayAnimation(DeathAnimation);
                break;
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (_animationPlayer != null && _animationPlayer.HasAnimation(animationName))
        {
            _animationPlayer.Play(animationName);
        }
        else
        {
            GD.PrintErr($"Animation {animationName} not found!");
        }
    }

    public override void _ExitTree()
    {
        if (_stateMachine != null)
        {
            _stateMachine.OnStateChanged -= HandleStateChanged;
        }
    }
}