using UnityEngine;

/// <summary>
/// Base class for all weapons / passive items. The base class is used so that both WeaponData
/// and PassiveItemData are able to be used interchangeably if required.
/// </summary>
public abstract class ItemData : ScriptableObject
{
    public Sprite icon;
    public string weaponName;
    public string weaponDescription;
    public int maxLevel;
    public float levelUpChance = 100; // Add this field to store the probability

    [System.Serializable]
    public struct Evolution
    {
        public string name;
        public enum Condition { auto, treasureChest }
        public Condition condition;

        [System.Flags] public enum Consumption { passives = 1, weapons = 2 }
        public Consumption consumes;

        public int evolutionLevel;
        public Config[] catalysts;
        public Config outcome;

        [System.Serializable]
        public struct Config
        {
            public ItemData itemType;
            public int level;
        }
    }

    public Evolution[] evolutionData;
    public abstract Skill.LevelData GetLevelData(int level);        
}
