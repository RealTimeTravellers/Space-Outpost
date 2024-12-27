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
    public Dictionary<AIState, IBaseState<AIState>> _states;
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

    public void ChangeState(AIState newState, Character character)
    {
        if (CurrentState == newState) return;
        
        AIState oldState = CurrentState;
        var characterName = character.Name ?? character.GetParent()?.Name;
        
        if (_states.ContainsKey(CurrentState))
            _states[CurrentState].Exit(character);
        
        CurrentState = newState;
        
        if (_states.ContainsKey(newState))
            _states[newState].Enter(character);
        
        OnStateChanged?.Invoke(oldState, newState);
    }

    public AIState ProcessState(Character aiCharacter)
    { 
        if (!_states.ContainsKey(CurrentState))
            return AIState.Patrol;
        
        return _states[CurrentState].Process(aiCharacter);
    }

}
