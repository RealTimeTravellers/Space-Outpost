using Godot;

public class EnemyAIController
{
    private EnemyAIStateMachine _stateMachine;

    public EnemyAIController()
    {
        // State geçişlerini yönetmek için state machine oluşturuyoruz
        _stateMachine = new EnemyAIStateMachine();
    }

    public void SetState(AIState newState, Enemy aiCharacter)
    {
        // Yeni state'e geçişi tetikle
        _stateMachine.ChangeState(newState, aiCharacter);
    }

    public void UpdateAI(Enemy aiCharacter, double delta)
    {
        // Mevcut state'in process metodunu çağır
        _stateMachine.UpdateCurrentState(aiCharacter, delta);
    }
}
