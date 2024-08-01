using UnityEngine;

/// <summary>
/// Replacement for the PassiveItemScriptableObject class. The idea is we want to store all 
/// passive item level data in one single object, instead of having multiple objects to store 
/// a single passive item, which is what we would have had to do if we continued using 
/// PassiveItemScriptableObject.
/// </summary>
[CreateAssetMenu(fileName = "Passive Data", menuName = "2D Top-down Rogue-like/Passive Data")]
public class PassiveData : ItemData
{
    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;
    public Passive.Modifier[] randomGrowth;

    public override Skill.LevelData GetLevelData(int level)
    {
        if (level <= 1) return baseStats;

        // Pick the stats from the next level.
        if (level - 2 < growth.Length)
            return growth[level - 2];
            
        // Otherwise, pick one of the stats from the random growth array.
        if (randomGrowth.Length > 0)
            return randomGrowth[Random.Range(0, randomGrowth.Length)];

        // Return an empty value and a warning.
        Debug.LogWarning(string.Format("Passive doesn't have its level up stats configured for Level {0}!", level));
        return new Passive.Modifier();
    }
}