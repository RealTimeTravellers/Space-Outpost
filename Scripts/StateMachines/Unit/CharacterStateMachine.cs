using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public enum CharacterStateType
{
    Idle,
    Moving,
    Aiming,
    Shooting,
    TakingCover,
    LeavingCover,
    Reloading,
    Death,
    Tactical
}

public class CharacterStateMachine
{
    private Dictionary<CharacterStateType, IBaseState<CharacterStateType>> _states;
    private IBaseState<CharacterStateType> _currentState;
    public CharacterStateType CurrentStateType { get; private set; }
    
    public event Action<CharacterStateType, CharacterStateType> OnStateChanged;

    public CharacterStateMachine()
    {
        _states = new Dictionary<CharacterStateType, IBaseState<CharacterStateType>>
        {
            { CharacterStateType.Idle, new CharacterIdleState() },
            { CharacterStateType.Moving, new CharacterMovingState() },
            { CharacterStateType.Aiming, new CharacterAimingState() },
            { CharacterStateType.Shooting, new CharacterShootingState() },
            { CharacterStateType.TakingCover, new CharacterTakingCoverState() },
            { CharacterStateType.LeavingCover, new CharacterLeavingCoverState() },
            { CharacterStateType.Reloading, new CharacterReloadingState() },
            { CharacterStateType.Death, new CharacterDeathState() },
            { CharacterStateType.Tactical, new CharacterTacticalState() }
        };

        CurrentStateType = CharacterStateType.Idle;
        _currentState = _states[CurrentStateType];
    }

    public void ChangeState(CharacterStateType newState, Character character)
    {
        if (!_states.ContainsKey(newState)) return;

        var oldState = CurrentStateType;
        _currentState?.Exit(character);
        CurrentStateType = newState;
        _currentState = _states[newState];
        _currentState.Enter(character);
        
        OnStateChanged?.Invoke(oldState, newState);
    }

    public void UpdateState(Character character)
    {
        var nextState = _currentState.Process(character);
        if (nextState != CurrentStateType)
        {
            ChangeState(nextState, character);
        }
    }
}