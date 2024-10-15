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

    public virtual AIState CheckState(Enemy enemy)
    {
        return AIState.Patrol;
    }

    protected bool PlayerInSight(Enemy enemy)
    {
        // Oyuncuyu görme mantığı burada uygulanmalı
        return false; // Şimdilik her zaman false dönüyor
    }

    protected bool EnemyInSight(Enemy enemy){
        // Etrafındaki düşmanları görme mantığı burada uygulanmalı
        return false; // Şimdilik her zaman false dönüyor
    }
}
