public abstract class BaseState : IBaseState
{
    public virtual void Enter(Enemy aiController)
    {
        
    }

    public virtual AIState Process(Enemy enemy)
    {
        return AIState.Patrol; 
    }

    public virtual void Exit(Enemy aiController)
    {
        
    }
}
