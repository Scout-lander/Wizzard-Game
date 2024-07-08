using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RuneRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic }
[CreateAssetMenu(fileName = "Rune Data New", menuName = "Inventory/New Rune Data")]
public class RuneDataNew : ScriptableObject
{
    public Sprite icon;
    public string gemName;
    public string description;
    public GemRarity rarity;

    [Header("Materials")]
    public Material commonMaterial; // Material for Common gems
    public Material uncommonMaterial; // Material for Uncommon gems
    public Material rareMaterial; // Material for Rare gems
    public Material epicMaterial; // Material for Epic gems
    public Material legendaryMaterial; // Material for Legendary gems
    public Material mythicMaterial; // Material for Mythic gems

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
        public int luck;
        public int curse;
        public float dashCount;
        public float dashCooldown;
        public int armor;
        public int gemHealth;
        public float lifeRegen;
        public int might;
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
}