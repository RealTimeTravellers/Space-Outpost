using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public abstract partial class Unit : Node3D
{
    public UnitStats Stats { get; protected set; }
    
    [Export]
    public string UnitName { get; set; }

    public override void _Ready()
    {
        InitializeStats();
    }

    protected virtual void InitializeStats()
    {
        GD.Print("Unit stats initialized");
    }

    public virtual void TakeTurn()
    {
        // Base implementation for taking a turn
    }

        public virtual void TakeDamage(int damage)
    {
        Stats.TakeDamage(damage);
        if (Stats.Health.GetValue() <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        GD.Print($"{UnitName} has died.");
        // Additional death logic here
    }

    public bool CanPerformAction()
    {
        return Stats.CanPerformAction();
    }

    public bool CalculateHitChance()
    {
        return Stats.CalculateHitChance();
    }

    public bool CalculateCriticalHitChance()
    {
        return Stats.CalculateCriticalHitChance();
    }

    public int GetMovementRange()
    {
        return Stats.GetMovementRange();
    }

    public void DecreaseMorale(int amount)
    {
        Stats.DecreaseMorale(amount);
    }
}
