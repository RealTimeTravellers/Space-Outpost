using Godot;

public abstract partial class Weapon : Equipment
{
    [Export] public int MaxAmmoClip { get; protected set; }
    [Export] public int CurrentAmmoClip { get; set; }
    [Export] public int Accuracy { get; protected set; }
    [Export] public int MaxDamage { get; protected set; }
    [Export] public int MinDamage { get; protected set; }
    [Export] public int Impact { get; protected set; }
    [Export] public int CritChance { get; protected set; }

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