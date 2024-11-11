using System;
using System.Collections.Generic;
using Godot;

public class Stat
{
    private int baseValue;
    private int currentValue;
    public List<int> modifiers = new List<int>();
    public int MinValue { get; private set; }
    public int MaxValue { get; private set; }

    // Event: Stat sıfırlandığında veya belirli bir seviyeye geldiğinde tetiklenir
    public event Action OnValueZero;

    public Stat(int minValue, int maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
        baseValue = Mathf.Clamp(baseValue, MinValue, MaxValue);
        currentValue = baseValue;
    }

    public int GetValue()
    {
        int finalValue = currentValue;

        foreach (int modifier in modifiers)
            finalValue += modifier;

        return Mathf.Clamp(finalValue, MinValue, MaxValue);
    }

    public void SetDefaultValue(int value)
    {
        baseValue = Mathf.Clamp(value, MinValue, MaxValue);
        currentValue = baseValue;
    }

    public void SetCurrentValue(int value)
    {
        currentValue = Mathf.Clamp(value, MinValue, MaxValue);
        CheckIfValueZero();
    }

    public void DecreaseValue(int amount)
    {
        currentValue = Mathf.Clamp(currentValue - amount, MinValue, MaxValue);
        CheckIfValueZero();
    }

    public void IncreaseValue(int amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, MinValue, MaxValue);
    }

    public void AddModifier(int modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifier(int modifier)
    {
        modifiers.Remove(modifier);
    }

    // Değer sıfırlandığında event tetikleyici
    private void CheckIfValueZero()
    {
        if (currentValue <= MinValue)
        {
            OnValueZero?.Invoke(); // Event'i tetikle
        }
    }
}
