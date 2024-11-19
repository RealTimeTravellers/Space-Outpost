public abstract class BaseState : IBaseState
{
    public virtual void Enter(Character aiController)
    {
        
    }

    public virtual AIState Process(Character enemy)
    {
        return AIState.Patrol; 
    }

    public virtual void Exit(Character aiController)
    {
        
    }

    public virtual AIState CheckState(Character enemy)
    {
        return AIState.Patrol;
    }

    protected bool PlayerInSight(Character enemy)
    {
        // Oyuncuyu görme mantığı burada uygulanmalı
        return false; // Şimdilik her zaman false dönüyor
    }

    protected bool EnemyInSight(Character enemy){
        // Etrafındaki düşmanları görme mantığı burada uygulanmalı
        return false; // Şimdilik her zaman false dönüyor
    }
}
