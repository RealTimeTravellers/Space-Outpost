using Godot;

public partial class EnemyAIController : Node
{
    public EnemyAIStateMachine _stateMachine {get; private set;}
    private Character _character;
    public bool _isActive = false;
    private bool _isBeingDestroyed = false;

    public override void _Ready()
    {
        base._Ready();
        _character = GetParent<Character>();
        _stateMachine = new EnemyAIStateMachine();
        TurnManager.Instance.TurnChanged += OnTurnChanged;
    }

    public override void _Process(double delta)
    {
        if (!_isActive || _character.CompletedTurn || _character.IsDead)
        {
            return;
        }

        _stateMachine.UpdateCurrentState(_character);
    }

    private void OnTurnChanged(bool isPlayerTurn)
    {
        if (!isPlayerTurn && _character != null && !_character.IsFriendly)
        {
            _isActive = true;
        }
        else
        {
            _isActive = false;
        }
    }

    public async void MoveToGrid(GridObject targetGrid)
    {
        if (targetGrid == null) return;
        await _character.Move(targetGrid);
    }

    public void SetState(AIState newState, Character aiCharacter)
    {
        _stateMachine.ChangeState(newState, aiCharacter);
    }

    public void PrepareForDestruction()
    {
        _isBeingDestroyed = true;
        _isActive = false;
        SetProcess(false);
    }
}