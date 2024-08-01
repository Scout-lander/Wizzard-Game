using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RuneRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic }
[CreateAssetMenu(fileName = "Rune Data", menuName = "Inventory/Rune Data")]
public class RuneDataNew : ScriptableObject
{
    public Sprite icon;
    public string iconName;
    public string gemName;
    public string description;
    public RuneRarity rarity;

    public bool takesDamage; 
    
    [Header("Probabilities")]
    public float commonProbabilities = 0.5f;
    public float uncommonProbabilities = 0.3f;
    public float rareProbabilities = 0.1f;
    public float epicProbabilities = 0.07f;
    public float legendaryProbabilities = 0.03f;
    public float mythicProbabilities = 0.00f;

    [System.Serializable]
    public class Stats
    {
        public int health;
        public float attackSpeed;
        public float luck;
        public float curse;
        public float dashCount;
        public float dashCooldown;
        public float armor;
        public float heartRune;
        public float lifeRegen;
        public float might;
        public float moveSpeed;
    }

    public Stats commonMinimumPossible;
    public Stats commonMaximumPossible;

    public Stats uncommonMinimumPossible;
    public Stats uncommonMaximumPossible;

    public Stats rareMinimumPossible;
    public Stats rareMaximumPossible;

    public Stats epicMinimumPossible;
    public Stats epicMaximumPossible;

    public Stats legendaryMinimumPossible;
    public Stats legendaryMaximumPossible;

    public Stats mythicMinimumPossible;
    public Stats mythicMaximumPossible;

    
[System.Serializable]
    public class MaxStats
    {
        public int health;
        public float attackSpeed;
        public int luck;
        public int curse;
        public float dashCount;
        public float dashCooldown;
        public float armor;
        public float heartRune;
        public float lifeRegen;
        public float might;
        public float moveSpeed;
    }

    public MaxStats maxStats; // Instance to hold max stat values
}