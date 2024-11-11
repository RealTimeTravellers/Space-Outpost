using Godot;

public partial class Accessory : Equipment
{
    [Export] public int UsageCount { get; set; }
    [Export] public bool IsPermanent { get; set; }
    [Export] public string Effect { get; set; }
    [Export] public bool IsPassive { get; set; }

    public override void ApplyEffects(UnitStats stats)
    {
        if (IsPassive)
        {
            // Apply passive effects here
        }
    }

    public override void RemoveEffects(UnitStats stats)
    {
        if (IsPassive)
        {
            // Remove passive effects here
        }
    }


    public virtual bool Use(Unit user, Unit target = null)
    {
        if (IsPassive)
        {
            return false; // Passive accessories can't be actively used
        }

        if (!IsPermanent && UsageCount <= 0)
        {
            return false; // Cannot use if out of uses
        }

        if (target != null && !IsGridOnSight(user, target))
        {
            return false; // Cannot use if target is not in sight
        }

        if (target != null && user.GetDistanceTo(target) > Range)
        {
            return false; // Cannot use if target is out of range
        }

        // Implement usage logic here

        if (!IsPermanent)
        {
            UsageCount--;
        }

        return true; // Successfully used
    }

    public virtual bool IsGridOnSight(Unit user, Unit target)
    {
        // This is a placeholder. Implement grid-based line of sight check here.
        return true;
    }

    
}