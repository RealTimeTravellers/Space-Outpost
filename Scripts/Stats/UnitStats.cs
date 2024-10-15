using Godot;

public enum UnitType
{
    Alien,
    Human
}


public partial class UnitStats : Resource
{
    [Export] public UnitType UnitType { get; set; } = UnitType.Human;
    public Stat Health { get; private set; }
    public Stat Armor { get; private set; }
    public Stat Accuracy { get; private set; }
    public Stat MovementRange { get; private set; }
    public Stat Morale { get; private set; }
    public Stat ActionPoints { get; private set; }
    public Stat Perception { get; private set; }
    public Stat Evasion { get; private set; }
    public Stat CriticalHitChance { get; private set; }

    public UnitStats()
    {
        Health = new Stat(0, 8);
        Armor = new Stat(0, 6);
        Accuracy = new Stat(30, 100);
        MovementRange = new Stat(0, 10);
        Morale = new Stat(0, 8);
        ActionPoints = new Stat(1, 3);
        Perception = new Stat(0, 100);
        Evasion = new Stat(0, 30);
        CriticalHitChance = new Stat(0, 100);

        // Set default values
        Health.SetDefaultValue(5);
        Armor.SetDefaultValue(3);
        Morale.SetDefaultValue(8);
    }

    public bool CalculateHitChance()
    {
        int hitChance = Accuracy.GetValue() - Evasion.GetValue(); 
        return GD.Randf() * 100 <= hitChance;
    }

    public int GetMovementRange()
    {
        return MovementRange.GetValue() * ActionPoints.GetValue();
    }

    public void DecreaseMorale(int amount)
    {
        Morale.DecreaseValue(amount);
        if (Morale.GetValue() == 0)
        {
            GD.Print("Morale is zero, switching to AI control.");
        }
    }

    public bool CanPerformAction()
    {
        return ActionPoints.GetValue() > 0;
    }

    public void DecreaseActionPoints(int amount = 1)
    {
        ActionPoints.DecreaseValue(amount);
    }

    public void DepleteActionPoints(int amount = 1)
    {
        ActionPoints.SetCurrentValue(0);
    }

    public bool CalculateCriticalHitChance()
    {
        return GD.Randf() * 100 <= CriticalHitChance.GetValue();
    }

    public void TakeDamage(int damage)
    {
        Health.DecreaseValue(damage);
        if (Health.GetValue() <= 0)
        {
            GD.Print("Unit has died.");
        }
    }

    // Methods to add and remove modifiers
    public void AddModifier(string statName, int modifier)
    {
        GetStatByName(statName)?.AddModifier(modifier);
    }

    public void RemoveModifier(string statName, int modifier)
    {
        GetStatByName(statName)?.RemoveModifier(modifier);
    }

    private Stat GetStatByName(string statName)
    {
        return statName switch
        {
            "Health" => Health,
            "Armor" => Armor,
            "Accuracy" => Accuracy,
            "MovementRange" => MovementRange,
            "Morale" => Morale,
            "ActionPoints" => ActionPoints,
            "Perception" => Perception,
            "Evasion" => Evasion,
            "CriticalHitChance" => CriticalHitChance,
            _ => null
        };
    }
}
