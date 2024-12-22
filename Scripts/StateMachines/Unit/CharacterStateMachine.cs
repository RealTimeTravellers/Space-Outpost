using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public enum CharacterStateType
{
    Idle,
    Moving,
    InCover,
    Aiming,
    Shooting,
    Death,
}

public class CharacterStateMachine
{
    private Dictionary<CharacterStateType, IBaseState<CharacterStateType>> _states;
    private IBaseState<CharacterStateType> _currentState;
    public CharacterStateType CurrentStateType { get; private set; }
    public CharacterStateType PreviousStateType { get; private set; }
    public event Action<CharacterStateType, CharacterStateType> OnStateChanged;
    public event Action<string> OnAnimationRequested; 

    public CharacterStateMachine()
    {
        _states = new Dictionary<CharacterStateType, IBaseState<CharacterStateType>>
        {
            { CharacterStateType.Idle, new CharacterIdleState() },
            { CharacterStateType.Moving, new CharacterMovingState() },
            { CharacterStateType.InCover, new CharacterInCoverState() },
            { CharacterStateType.Aiming, new CharacterAimingState() },
            { CharacterStateType.Shooting, new CharacterShootingState() },
            { CharacterStateType.Death, new CharacterDeathState() },
        };

        CurrentStateType = CharacterStateType.Idle;
        _currentState = _states[CurrentStateType];
    }

    public void ChangeState(CharacterStateType newState, Character character)
    {
        if (!_states.ContainsKey(newState)) return;

        PreviousStateType = CurrentStateType;
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

    public void RequestAnimation(string animationName)
    {
        OnAnimationRequested?.Invoke(animationName);
    }
}