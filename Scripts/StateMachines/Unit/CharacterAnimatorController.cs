using Godot;

public partial class CharacterAnimatorController : Node
{
    private AnimationPlayer _animationPlayer;
    public AnimationTree _animationTree {get; private set;}
    [Export] public Character _character;
    [Export] public CharacterController _characterController;
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
        _animationTree.Set("parameters/conditions/incover", false);
        _animationTree.Set("parameters/conditions/outcover", false);
        _animationTree.Set("parameters/conditions/aiming", false);
        _animationTree.Set("parameters/conditions/death", false);
    }

    private void HandleStateChanged(CharacterStateType oldState, CharacterStateType newState)
    {
        if (_animationTree == null) return;
        
        string stateName = newState.ToString().ToLowerInvariant();
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
            case "shooting":
                _animationTree.Set("parameters/conditions/shooting", true);
                GD.Print("[Animation] Setting shooting true");
                break;
            case "incover":
                _animationTree.Set("parameters/conditions/incover", true);
                GD.Print("[Animation] Setting in_cover true");
                break;
            case "outcover":
                _animationTree.Set("parameters/conditions/outcover", true);
                GD.Print("[Animation] Setting out_cover true");
                break;
            case "aiming":
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
        
    public bool IsAnimationPlaying(string animationName)
    {
        if (_animationTree == null) return false;
        
        // Animasyon ağacından shooting condition'ını kontrol et
        bool isPlaying = _animationTree.Get($"parameters/conditions/{animationName}").AsBool();
        return isPlaying;
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