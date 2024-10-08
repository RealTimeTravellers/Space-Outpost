using System.Collections.Generic;

public enum AIState
{
    Patrol,
    Alert,
    Aggression,
    Flee,
    Cower,
    Tactical
}

public class EnemyAIStateMachine
{
    private Dictionary<AIState, IBaseState> _states;
    private IBaseState _currentState;

    public EnemyAIStateMachine()
    {
        _states = new Dictionary<AIState, IBaseState>
        {
            { AIState.Patrol, new PatrolState() },
            { AIState.Alert, new AlertState() },
            { AIState.Aggression, new AggressionState() },
            { AIState.Flee, new FleeState() },
            { AIState.Cower, new CowerState() },
            { AIState.Tactical, new TacticalState() }
        };

        _currentState = _states[AIState.Patrol]; 
    }

    public void ChangeState(AIState newState, Enemy aiCharacter)
    {
        // Mevcut state'den çık ve yeni state'e geçiş yap
        _currentState.Exit(aiCharacter);
        _currentState = _states[newState];
        _currentState.Enter(aiCharacter);
    }

    public void UpdateCurrentState(Enemy aiCharacter, double delta)
    {
        // Mevcut state'in process metodunu çağır
        _currentState.Process(aiCharacter, delta);
    }
}
