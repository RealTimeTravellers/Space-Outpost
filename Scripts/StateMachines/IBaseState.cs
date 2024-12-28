using System;
using System.Threading.Tasks;
public interface IBaseState<TStateType> where TStateType : Enum
{
    void Enter(Character character);
    TStateType Process(Character character);
    TStateType CheckState(Character character);
    void Exit(Character character);
    Task Decide(Character character);
}