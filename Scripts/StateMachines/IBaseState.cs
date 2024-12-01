using System;

public interface IBaseState<TStateType> where TStateType : Enum
{
    void Enter(Character character);
    TStateType Process(Character character);
    void Exit(Character character);
}