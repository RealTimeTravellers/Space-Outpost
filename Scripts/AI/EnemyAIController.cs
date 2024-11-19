using Godot;

public partial class EnemyAIController : Node
{
    private EnemyAIStateMachine _stateMachine;

    public EnemyAIController()
    {
        // State geçişlerini yönetmek için state machine oluşturuyoruz
        _stateMachine = new EnemyAIStateMachine();
        _stateMachine.OnStateChanged += OnStateChanged;
    }

    public void SetState(AIState newState, Character aiCharacter)
    {
        // Yeni state'e geçişi tetikle
        _stateMachine.ChangeState(newState, aiCharacter);
    }

    public void UpdateAI(Character aiCharacter)
    {
        AIState currentState = _stateMachine.CurrentState;
        AIState nextState = _stateMachine.UpdateCurrentState(aiCharacter);
        if (nextState != currentState)
        {
            SetState(nextState, aiCharacter);
        }
    }

    private void OnStateChanged(AIState oldState, AIState newState)
    {
        GD.Print($"State changed from {oldState} to {newState}");
    }
}
