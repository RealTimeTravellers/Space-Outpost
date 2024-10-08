public interface IBaseState
{
    void Enter(Enemy enemy);
    void Process(Enemy enemy, double delta);
    void Exit(Enemy enemy);
}