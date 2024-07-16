using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuneBagSerializable
{
    public int maxCapacity;
    public List<Rune> rune = new List<Rune>();

    public void AddRune(Rune newRune)
    {
        if (rune.Count < maxCapacity)
        {
            rune.Add(newRune);
        }
    }

    public void RemoveRune(Rune oldRune)
    {
        rune.Remove(oldRune);
    }
}