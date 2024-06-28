using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for both the Passive and the Weapon classes. It is primarily intended
/// to handle weapon evolution, as we want both weapons and passives to be evolve-able.
/// </summary>
public abstract class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;
    [HideInInspector] public ItemData data;
    protected ItemData.Evolution[] evolutionData;
    protected PlayerInventory inventory;
    protected PlayerStats owner;

    public PlayerStats Owner { get { return owner; } }

    [System.Serializable]
    public class LevelData
    {
        public string name, description;
    }

    public virtual void Initialise(ItemData data)
    {
        maxLevel = data.maxLevel;
        this.data = data; // Store the data reference
        evolutionData = data.evolutionData;
        inventory = GetComponentInParent<PlayerInventory>();
        owner = GetComponentInParent<PlayerStats>();
    }

    public virtual ItemData.Evolution[] CanEvolve()
    {
        List<ItemData.Evolution> possibleEvolutions = new List<ItemData.Evolution>();

        foreach (ItemData.Evolution e in evolutionData)
        {
            if (CanEvolve(e)) possibleEvolutions.Add(e);
        }

        return possibleEvolutions.ToArray();
    }

    public virtual bool CanEvolve(ItemData.Evolution evolution, int levelUpAmount = 1)
    {
        if (evolution.evolutionLevel > currentLevel + levelUpAmount)
        {
            Debug.LogWarning(string.Format("Evolution failed. Current level {0}, evolution level {1}", currentLevel, evolution.evolutionLevel));
            return false;
        }

        foreach (ItemData.Evolution.Config c in evolution.catalysts)
        {
            Item item = inventory.Get(c.itemType);
            if (!item || item.currentLevel < c.level)
            {
                Debug.LogWarning(string.Format("Evolution failed. Missing {0}", c.itemType.name));
                return false;
            }
        }

        return true;
    }

    public virtual bool AttemptEvolution(ItemData.Evolution evolutionData, int levelUpAmount = 1)
    {
        if (!CanEvolve(evolutionData, levelUpAmount))
            return false;

        bool consumePassives = (evolutionData.consumes & ItemData.Evolution.Consumption.passives) > 0;
        bool consumeWeapons = (evolutionData.consumes & ItemData.Evolution.Consumption.weapons) > 0;

        foreach (ItemData.Evolution.Config c in evolutionData.catalysts)
        {
            if (c.itemType is PassiveData && consumePassives) inventory.Remove(c.itemType, true);
            if (c.itemType is WeaponData && consumeWeapons) inventory.Remove(c.itemType, true);
        }

        if (this is Passive && consumePassives) inventory.Remove((this as Passive).data, true);
        else if (this is Weapon && consumeWeapons) inventory.Remove((this as Weapon).data, true);

        inventory.Add(evolutionData.outcome.itemType);

        return true;
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;
    }

    public virtual bool DoLevelUp()
    {
        if (evolutionData == null) return true;

        foreach (ItemData.Evolution e in evolutionData)
        {
            if (e.condition == ItemData.Evolution.Condition.auto)
                AttemptEvolution(e);
        }

        // Check the level-up chance
        if (Random.value <= data.levelUpChance)
        {
            // Logic for adding the item as a level-up option
            Debug.Log($"Item {data.weaponName} has been added as a level-up option.");
            // You can add your specific logic here to handle the level-up option.
        }

        return true;
    }

    public virtual void OnEquip() { }

    public virtual void OnUnequip() { }
}
