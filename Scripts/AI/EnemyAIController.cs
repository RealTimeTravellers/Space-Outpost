using Godot;

public class EnemyAIController
{
    private EnemyAIStateMachine _stateMachine;

    public EnemyAIController()
    {
        // State geçişlerini yönetmek için state machine oluşturuyoruz
        _stateMachine = new EnemyAIStateMachine();
        _stateMachine.OnStateChanged += OnStateChanged;
    }

    public void SetState(AIState newState, Enemy aiCharacter)
    {
        // Yeni state'e geçişi tetikle
        _stateMachine.ChangeState(newState, aiCharacter);
    }

    public void UpdateAI(Enemy aiCharacter)
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
