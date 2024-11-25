using System;

public abstract class BaseState<TStateType> : IBaseState<TStateType> where TStateType : Enum
{
    public virtual void Enter(Character character)
    {
        
    }

    public virtual TStateType Process(Character character)
    {
        return default;
    }

    public virtual void Exit(Character character)
    {
        
    }

    public virtual TStateType CheckState(Character character)
    {
        return default;
    }

    protected bool PlayerInSight(Character character)
    {
        // Oyuncuyu görme mantığı burada uygulanmalı
        return false;
    }

    protected bool EnemyInSight(Character character)
    {
        // Etrafındaki düşmanları görme mantığı burada uygulanmalı
        return false;
    }
}