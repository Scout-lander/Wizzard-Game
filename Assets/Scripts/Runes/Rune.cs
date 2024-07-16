using UnityEngine;

[System.Serializable]
public class Rune
{
    public Sprite icon;
    public string name;
    public string description;
    public RuneRarity rarity;
    public RuneDataNew.Stats actualStats;
    public bool takesDamage; 
}
