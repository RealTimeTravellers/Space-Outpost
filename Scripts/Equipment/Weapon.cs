using System;
using Godot;

public abstract partial class Weapon : Equipment
{
    [Export] public int MaxAmmoClip { get; protected set; }
    [Export] public int Accuracy { get; protected set; }
    [Export] public int MaxDamage { get; protected set; }
    [Export] public int MinDamage { get; protected set; }
    [Export] public int Impact { get; protected set; }
    [Export] public int CritChance { get; protected set; }

    private int _currentAmmoClip;
    public int CurrentAmmoClip 
    { 
        get => _currentAmmoClip;
        private set => _currentAmmoClip = Mathf.Clamp(value, 0, MaxAmmoClip);
    }

    protected void Initialize(WeaponStatsContainer stats)
    {
        Name = stats.Name;
        Description = stats.Description;
        MaxAmmoClip = stats.MaxAmmoClip;
        CurrentAmmoClip = MaxAmmoClip;
        Accuracy = stats.Accuracy;
        Range = stats.Range;
        MinDamage = stats.MinDamage;
        MaxDamage = stats.MaxDamage;
        Impact = stats.Impact;
        CritChance = stats.CritChance;
        Icon = stats.Icon;
    }
    public virtual void Reload()
    {
        CurrentAmmoClip = MaxAmmoClip;
    }

    public virtual bool NeedsReload()
    {
        return CurrentAmmoClip <= 0;
    }

    public virtual void Fire()
    {
        if (CurrentAmmoClip > 0)
        {
            CurrentAmmoClip--;
        }
    }

    public virtual int DealDamage(bool isCritical = false)
    {
        int damageDealt = GD.RandRange(MinDamage, MaxDamage);
        if (isCritical)
        {
            damageDealt *= 2;
        }
        return damageDealt;
    }
}