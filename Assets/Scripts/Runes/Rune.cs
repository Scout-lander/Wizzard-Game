using UnityEngine;

[System.Serializable]
public class Rune
{
     //public Sprite icon;
    public string iconName; // Store the sprite name instead of the sprite itself
    
    public string name;
    public string description;
    public RuneRarity rarity;
    public RuneDataNew.Stats actualStats;
    public bool takesDamage; 

    public void SetIcon(Sprite icon)
    {
        iconName = icon.name; // Save the name of the sprite
    }

    public Sprite GetIcon()
    {
        return Resources.Load<Sprite>("RuneIcons/" + iconName); // Load the sprite from Resources
    }
}
