using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Stat
{
    [SerializeField] private int baseValue;

    [SerializeField] private List<int> Modifiers = new List<int>();

    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (int modifier in Modifiers)
        {
            finalValue += modifier;
        }
        return finalValue;
    }
    
    public int Value {
        get
        {
            int finalValue = baseValue;

            foreach (int modifier in Modifiers)
            {
                finalValue += modifier;
            }
            return finalValue;
        }
    }

    public void SetDefaultValue(int value)
    {
        baseValue = value;
    }

    public void AddModifier(int modifier)
    {
        Modifiers.Add(modifier);
    }

    public void RemoveModifier(int modifier)
    {
        Modifiers.Remove(modifier);
    }
}
