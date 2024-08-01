using System.Collections.Generic;
using UnityEngine;

public class RuneInventory : MonoBehaviour
{
    [Header("Do not edit the Runes. It is updated by scripts.")]
    [Header("You can only update the Capacity.")]
    [Header("")]
    public RuneBagSerializable runeBag = new RuneBagSerializable();
    public RuneBagSerializable equippedRuneBag = new RuneBagSerializable();
    
    [Header("All possible rune data.")]
    public List<RuneDataNew> allRuneData; // A list of all possible rune data

    private SaveLoadManager saveLoadManager;

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        ValidateRunes();
    }

    private void ValidateRunes()
    {
        for (int i = runeBag.runes.Count - 1; i >= 0; i--)
        {
            Rune rune = runeBag.runes[i];
            RuneDataNew runeData = FindRuneData(rune.name);
            if (runeData == null)
            {
                Debug.LogWarning($"Rune data not found for rune: {rune.name}");
                continue;
            }

            if (!IsRuneValid(rune, runeData))
            {
                Debug.Log($"Stop Cheating!");
                Debug.Log($"Invalid rune detected and destroyed: {rune.name}");
                DestroyRune(runeBag, i);
            }
        }

        Debug.Log("Rune validation complete.");
    }

    public RuneDataNew FindRuneData(string runeName)
    {
        foreach (RuneDataNew runeData in allRuneData)
        {
            if (runeData.gemName == runeName)
            {
                return runeData;
            }
        }
        return null;
    }

    private bool IsRuneValid(Rune rune, RuneDataNew runeData)
    {
        RuneDataNew.MaxStats maxStats = runeData.maxStats;

        return rune.actualStats.health <= maxStats.health &&
               rune.actualStats.attackSpeed <= maxStats.attackSpeed &&
               rune.actualStats.luck <= maxStats.luck &&
               rune.actualStats.curse <= maxStats.curse &&
               rune.actualStats.dashCount <= maxStats.dashCount &&
               rune.actualStats.dashCooldown <= maxStats.dashCooldown &&
               rune.actualStats.armor <= maxStats.armor &&
               rune.actualStats.heartRune <= maxStats.heartRune &&
               rune.actualStats.lifeRegen <= maxStats.lifeRegen &&
               rune.actualStats.might <= maxStats.might &&
               rune.actualStats.moveSpeed <= maxStats.moveSpeed;
    }

    private void DestroyRune(RuneBagSerializable runeBag, int index)
    {
        runeBag.runes.RemoveAt(index);
        SaveRuneBags();
    }

    private void SaveRuneBags()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.SaveRuneBags(runeBag, equippedRuneBag);
        }
    }

    public static RuneStats GenerateRuneStatsForRarity(RuneDataNew runeData, RuneRarity rarity)
    {
        RuneDataNew.Stats minStats;
        RuneDataNew.Stats maxStats;

        // Determine the stat ranges based on the rarity
        switch (rarity)
        {
            case RuneRarity.Common:
                minStats = runeData.commonMinimumPossible;
                maxStats = runeData.commonMaximumPossible;
                break;
            case RuneRarity.Uncommon:
                minStats = runeData.uncommonMinimumPossible;
                maxStats = runeData.uncommonMaximumPossible;
                break;
            case RuneRarity.Rare:
                minStats = runeData.rareMinimumPossible;
                maxStats = runeData.rareMaximumPossible;
                break;
            case RuneRarity.Epic:
                minStats = runeData.epicMinimumPossible;
                maxStats = runeData.epicMaximumPossible;
                break;
            case RuneRarity.Legendary:
                minStats = runeData.legendaryMinimumPossible;
                maxStats = runeData.legendaryMaximumPossible;
                break;
            case RuneRarity.Mythic:
                minStats = runeData.mythicMinimumPossible;
                maxStats = runeData.mythicMaximumPossible;
                break;
            default:
                return new RuneStats();
        }

        // Generate stats within the specified range
        RuneStats newStats = new RuneStats
        {
            health = Random.Range(minStats.health, maxStats.health),
            attackSpeed = Random.Range(minStats.attackSpeed, maxStats.attackSpeed),
            luck = Random.Range(minStats.luck, maxStats.luck),
            curse = Random.Range(minStats.curse, maxStats.curse),
            dashCount = Random.Range(minStats.dashCount, maxStats.dashCount),
            dashCooldown = Random.Range(minStats.dashCooldown, maxStats.dashCooldown),
            armor = Random.Range(minStats.armor, maxStats.armor),
            heartRune = Random.Range(minStats.heartRune, maxStats.heartRune),
            lifeRegen = Random.Range(minStats.lifeRegen, maxStats.lifeRegen),
            might = Random.Range(minStats.might, maxStats.might),
            moveSpeed = Random.Range(minStats.moveSpeed, maxStats.moveSpeed)
        };

        return newStats;
    }
}
