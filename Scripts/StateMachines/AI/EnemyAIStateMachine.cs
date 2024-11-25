using System;
using System.Collections.Generic;
using Godot;

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
    private Dictionary<AIState, IBaseState<AIState>> _states;
    public event Action<AIState, AIState> OnStateChanged;
    public AIState CurrentState { get; private set; }

    public EnemyAIStateMachine()
    {
        _states = new Dictionary<AIState, IBaseState<AIState>>
        {
            { AIState.Patrol, new PatrolState() },
            { AIState.Alert, new AlertState() },
            { AIState.Aggression, new AggressionState() },
            { AIState.Flee, new FleeState() },
            { AIState.Cower, new CowerState() },
            { AIState.Tactical, new TacticalState() }
        };

        CurrentState = AIState.Patrol;
    }



    public void ChangeState(AIState newState, Character aiCharacter)
    {
        AIState oldState = CurrentState;
        _states[CurrentState].Exit(aiCharacter);
        CurrentState = newState;
        _states[CurrentState].Enter(aiCharacter);
        
        OnStateChanged?.Invoke(oldState, newState);
    }

    public AIState UpdateCurrentState(Character aiCharacter)
    {
        if (!_states.ContainsKey(CurrentState))
        {
            GD.PrintErr($"Invalid state: {CurrentState}");
            return AIState.Patrol; // Varsayılan duruma dön
        }
        return _states[CurrentState].Process(aiCharacter);
    }
}
