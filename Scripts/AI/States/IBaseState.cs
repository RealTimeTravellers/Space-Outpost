public interface IBaseState
{
    void Enter(Enemy enemy);
    AIState Process(Enemy enemy);
    void Exit(Enemy enemy);
}