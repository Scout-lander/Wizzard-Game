using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuneBagSerializable
{
    public int maxCapacity;
    public List<Rune> runes = new List<Rune>();

    public void AddRune(Rune newRune)
    {
        if (runes.Count < maxCapacity)
        {
            runes.Add(newRune);
        }
    }

    public void RemoveRune(Rune oldRune)
    {
        runes.Remove(oldRune);
    }

    public void InitializeIcons()
    {
        foreach (var rune in runes)
        {
            rune.GetIcon(); // Ensure the icon is loaded
        }
    }
}
