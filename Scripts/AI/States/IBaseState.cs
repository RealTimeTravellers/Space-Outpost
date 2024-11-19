public interface IBaseState
{
    void Enter(Character enemy);
    AIState Process(Character enemy);
    void Exit(Character enemy);
}