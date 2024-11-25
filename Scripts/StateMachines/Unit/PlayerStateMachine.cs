using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public enum PlayerStateType
{
    Idle,
    Moving,
    Aiming,
    Shooting,
    TakingCover,
    LeavingCover,
    Reloading,
    Death
}

public class PlayerStateMachine
{
    private Dictionary<PlayerStateType, IBaseState<PlayerStateType>> _states;
    private IBaseState<PlayerStateType> _currentState;
    public PlayerStateType CurrentStateType { get; private set; }
    
    public event Action<PlayerStateType, PlayerStateType> OnStateChanged;

    public PlayerStateMachine()
    {
        _states = new Dictionary<PlayerStateType, IBaseState<PlayerStateType>>
        {
            { PlayerStateType.Idle, new PlayerIdleState() },
            { PlayerStateType.Moving, new PlayerMovingState() },
            { PlayerStateType.Aiming, new PlayerAimingState() },
            { PlayerStateType.Shooting, new PlayerShootingState() },
            { PlayerStateType.TakingCover, new PlayerTakingCoverState() },
            { PlayerStateType.LeavingCover, new PlayerLeavingCoverState() },
            { PlayerStateType.Reloading, new PlayerReloadingState() },
            { PlayerStateType.Death, new PlayerDeathState() }
        };

        CurrentStateType = PlayerStateType.Idle;
        _currentState = _states[CurrentStateType];
    }

    public void ChangeState(PlayerStateType newState, Character character)
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