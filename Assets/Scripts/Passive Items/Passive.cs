using UnityEngine;

/// <summary>
/// A class that takes a PassiveData and is used to increment a player's
/// stats when received.
/// </summary>
public class Passive : Item
{
    [SerializeField] private PlayerStats.Stats currentBoosts;

    [System.Serializable]
    public class Modifier : LevelData
    {
        public PlayerStats.Stats boosts;
    }

    // For dynamically created passives, call initialise to set everything up.
    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual PlayerStats.Stats GetBoosts()
    {
        return currentBoosts;
    }

    // Levels up the weapon by 1, and calculates the corresponding stats.
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        // Prevent level up if we are already at max level.
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to Level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        // Otherwise, add stats of the next level to our weapon.
        currentBoosts += ((Modifier)data.GetLevelData(++currentLevel)).boosts;
        return true;
    }
}