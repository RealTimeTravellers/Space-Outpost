using Godot;

public partial class CharacterAnimatorController : Node
{
    private AnimationPlayer _animationPlayer;
    public AnimationTree _animationTree {get; private set;}
    private Character _character;
    private CharacterController _characterController;
    private CharacterStateMachine _stateMachine;

    public override void _Ready()
    {
        _character = GetParent<Character>();
        _characterController = _character.GetNode<CharacterController>("CharacterController");
        _stateMachine = _characterController._stateMachine;

        var askerNode = GetNode<Node3D>("Asker");
        _animationPlayer = askerNode.GetNode<AnimationPlayer>("AnimationPlayer");
        _animationTree = askerNode.GetNode<AnimationTree>("AnimationTree");

        if (_animationTree != null)
        {
            _animationTree.Active = true;
            GD.Print("[Animation] AnimationTree activated");
        }
        else
        {
            GD.PrintErr("[Animation] AnimationTree node not found!");
        }

        if (_character != null && _stateMachine != null)
        {
            _stateMachine.OnStateChanged += HandleStateChanged;
            _stateMachine.OnAnimationRequested += HandleAnimationRequested;
        }
    }

    private void ResetAllAnimationStates()
    {
        _animationTree.Set("parameters/conditions/idle", false);
        _animationTree.Set("parameters/conditions/moving", false);
        _animationTree.Set("parameters/conditions/shooting", false);
        _animationTree.Set("parameters/conditions/in_cover", false);
        _animationTree.Set("parameters/conditions/aiming", false);
        _animationTree.Set("parameters/conditions/death", false);
    }

    private void HandleStateChanged(CharacterStateType oldState, CharacterStateType newState)
    {
        if (_animationTree == null) return;
        
        string stateName = newState.ToString().ToLower();
        GD.Print($"[Animation] State changed to: {stateName}");
        
        // State değişiminde animasyon isteğini tetikle
        HandleAnimationRequested(stateName);
    }
    private void HandleAnimationRequested(string animationName)
    {
        if (_animationTree == null) return;

        ResetAllAnimationStates();

        switch (animationName)
        {
            case "idle":
                _animationTree.Set("parameters/conditions/idle", true);
                GD.Print("[Animation] Setting idle true");
                break;
            case "moving":
                _animationTree.Set("parameters/conditions/moving", true);
                GD.Print("[Animation] Setting moving true");
                break;
            case "shoot":
                _animationTree.Set("parameters/conditions/shooting", true);
                GD.Print("[Animation] Setting shooting true");
                break;
            case "in_cover":
                _animationTree.Set("parameters/conditions/in_cover", true);
                GD.Print("[Animation] Setting in_cover true");
                break;
            case "aim":
                _animationTree.Set("parameters/conditions/aiming", true);
                GD.Print("[Animation] Setting aiming true");
                break;
            case "death":
                _animationTree.Set("parameters/conditions/death", true);
                GD.Print("[Animation] Setting death true");
                break;
            default:
                GD.PrintErr($"Unknown animation requested: {animationName}");
                break;
        }
    }


    private void PlayAnimationWithBlend(string animationName, bool shouldLoop = true)
    {
        if (_animationPlayer != null)
        {
            // Animasyonu durdur
            _animationPlayer.Stop();
            
            // Loop modunu ayarla
            var animation = _animationPlayer.GetAnimation(animationName);
            if (animation != null)
            {
                animation.LoopMode = shouldLoop ? Animation.LoopModeEnum.Linear : Animation.LoopModeEnum.None;
            }
            
            // Animasyonu başlat
            _animationPlayer.Play(animationName);
            GD.Print($"Playing animation: {animationName}, Loop: {shouldLoop}");
        }
        else
        {
            GD.PrintErr("AnimationPlayer is null!");
        }
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
            _stateMachine.OnAnimationRequested -= HandleAnimationRequested; 
        }
    }
}