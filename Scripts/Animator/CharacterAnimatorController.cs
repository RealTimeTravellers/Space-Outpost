using Godot;

public partial class CharacterAnimatorController : Node
{
    private AnimationPlayer _animationPlayer;
    private Character _character;
    private CharacterStateMachine _stateMachine;
    
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
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _character = GetParent<Character>();
        _stateMachine = _character.GetNode<CharacterController>("CharacterController")._stateMachine;
        
        if (_character != null)
        {
            if (_stateMachine != null)
            {
                _stateMachine.OnStateChanged += HandleStateChanged;
            }
        }
    }

    private void HandleStateChanged(CharacterStateType oldState, CharacterStateType newState)
    {
        switch (newState)
        {
            case CharacterStateType.Idle:
                PlayAnimation(IdleAnimation);
                break;
            case CharacterStateType.Shooting:
                PlayAnimation(ShootingAnimation);
                break;
            case CharacterStateType.Reloading:
                PlayAnimation(ReloadingAnimation);
                break;
            case CharacterStateType.Moving:
                PlayAnimation(MovingAnimation);
                break;
            case CharacterStateType.TakingCover:
                PlayAnimation(TakingCoverAnimation);
                break;
            case CharacterStateType.LeavingCover:
                PlayAnimation(LeavingCoverAnimation);
                break;
            case CharacterStateType.Aiming:
                PlayAnimation(AimingAnimation);
                break;
            case CharacterStateType.Death:
                PlayAnimation(DeathAnimation);
                break;
            case CharacterStateType.Tactical:
                PlayAnimation(TacticalStanceAnimation);
                break;
        }
    }

    protected void PlayAnimation(string animationName)
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