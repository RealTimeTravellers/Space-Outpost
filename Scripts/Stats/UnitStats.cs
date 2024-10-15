using System.Collections.Generic;
using Godot;

public enum UnitType
{
    Alien,
    Human
}


public class UnitStats
{

    public UnitType unitType;    
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
        unitType = UnitType.Human;
        Health = new Stat(0, 8);
        Armor = new Stat(0, 6);
        Accuracy = new Stat(30, 100);
        MovementRange = new Stat(0, 10);
        Morale = new Stat(0, 8);
        ActionPoints = new Stat(1, 3);
        Perception = new Stat(0, 100);
        Evasion = new Stat(0, 30);
        CriticalHitChance = new Stat(0, 100);

        // Varsayılan değerleri ayarlayalım
        Health.SetDefaultValue(5);
        Armor.SetDefaultValue(3);
        Morale.SetDefaultValue(8);
    }

    public bool CalculateHitChance()
    {
        int hitChance = Accuracy.GetValue() - Evasion.GetValue(); 
        return GD.Randf() * 100 <= hitChance;
    }

    // Hareket menzili hesaplama
    public int GetMovementRange()
    {
        return MovementRange.GetValue() * ActionPoints.GetValue();
    }

    // Moral sıfırlandığında olay fırlatma
    public void DecreaseMorale(int amount)
    {
        Morale.DecreaseValue(amount);
        if (Morale.GetValue() == 0)
        {
            // Moral sıfırlandı, AI devreye girebilir
            GD.Print("Morale is zero, switching to AI control.");
        }
    }

    // Aksiyon puanı kontrolü
    public bool CanPerformAction()
    {
        return ActionPoints.GetValue() > 0; // Eğer aksiyon puanı varsa aksiyon yapılabilir
    }

    // Kritik vuruş şansı hesaplama
    public bool CalculateCriticalHitChance()
    {
        return GD.Randf() * 100 <= CriticalHitChance.GetValue(); // Kritik vuruş true/false
    }

    // Hasar almak (Health değerini azaltır)
    public void TakeDamage(int damage)
    {
        Health.DecreaseValue(damage);
        if (Health.GetValue() <= 0)
        {
            GD.Print("Unit has died.");
        }
    }
}