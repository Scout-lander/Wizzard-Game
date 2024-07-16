using UnityEngine;

public class RunePickup : Pickup
{
    protected override void Start()
    {
        base.Start();
        if (pickupType != PickupType.Rune)
        {
            Debug.LogWarning("RunePickup script is used, but PickupType is not set to Rune.");
        }
    }

    public override bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if (pickupType == PickupType.Rune)
        {
            // Ensure that the target is only set if it hasn't already been set.
            if (!this.target)
            {
                this.target = target;
                this.speed = speed;
                if (lifespan > 0) this.lifespan = lifespan;

                if (target == null)
                {
                    Debug.LogWarning("Target is null. Aborting rune collection.");
                    return false;
                }

                RuneInventory runeInventory = target.GetComponent<RuneInventory>();
                if (runeInventory != null)
                {
                    Debug.Log("Rune bag current capacity: " + runeInventory.runeBag.rune.Count + "/" + runeInventory.runeBag.maxCapacity);

                    if (runeInventory.runeBag.rune.Count < runeInventory.runeBag.maxCapacity)
                    {
                        Rune rune = ConvertToRuneNew(runeDataNew);
                        runeInventory.runeBag.rune.Add(rune);
                    }
                    else
                    {
                        Debug.Log("Rune bag is full. Cannot collect more runes.");
                    }
                }
                else
                {
                    Debug.LogWarning("Target does not have a RuneInventoryNew component.");
                }

                Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));  // Destroy the pickup object
                return true;
            }
            else
            {
                Debug.Log("Target is already set. Rune collection aborted.");
            }
        }
        else
        {
            Debug.LogWarning("RunePickup.Collect called, but PickupType is not Rune.");
        }

        return false;
    }

    private Rune ConvertToRuneNew(RuneDataNew runeDataNew)
    {
        // Roll for the rarity based on probabilities
        RuneRarity rolledRarity = RollForRarity(runeDataNew);

        // Create and initialize the new rune with the stats from the rolled rarity
        Rune newRune = new Rune();
        newRune.icon = runeDataNew.icon;
        newRune.name = runeDataNew.gemName;
        newRune.description = runeDataNew.description;
        newRune.rarity = rolledRarity;

        // Set stats based on the rolled rarity
        newRune.actualStats = InitializeRuneStats(runeDataNew, rolledRarity);

        Debug.Log("New rune created with name: " + newRune.name + " and rarity: " + newRune.rarity);
        return newRune;
    }

    private RuneRarity RollForRarity(RuneDataNew runeDataNew)
    {
        // Get the probabilities
        float[] probabilities = {
            runeDataNew.commonProbabilities,
            runeDataNew.uncommonProbabilities,
            runeDataNew.rareProbabilities,
            runeDataNew.epicProbabilities,
            runeDataNew.legendaryProbabilities,
            runeDataNew.mythicProbabilities
        };

        float roll = Random.Range(0f, 1f);
        float cumulative = 0f;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (roll < cumulative)
            {
                return (RuneRarity)i;
            }
        }

        return RuneRarity.Common;  // Default to common if something goes wrong
    }

    private RuneDataNew.Stats InitializeRuneStats(RuneDataNew runeDataNew, RuneRarity rarity)
    {
        RuneDataNew.Stats minStats;
        RuneDataNew.Stats maxStats;

        // Set the min and max stats based on the rarity
        switch (rarity)
        {
            case RuneRarity.Common:
                minStats = runeDataNew.commonMinimumPossible;
                maxStats = runeDataNew.commonMaximumPossible;
                break;
            case RuneRarity.Uncommon:
                minStats = runeDataNew.uncommonMinimumPossible;
                maxStats = runeDataNew.uncommonMaximumPossible;
                break;
            case RuneRarity.Rare:
                minStats = runeDataNew.rareMinimumPossible;
                maxStats = runeDataNew.rareMaximumPossible;
                break;
            case RuneRarity.Epic:
                minStats = runeDataNew.epicMinimumPossible;
                maxStats = runeDataNew.epicMaximumPossible;
                break;
            case RuneRarity.Legendary:
                minStats = runeDataNew.legendaryMinimumPossible;
                maxStats = runeDataNew.legendaryMaximumPossible;
                break;
            case RuneRarity.Mythic:
                minStats = runeDataNew.mythicMinimumPossible;
                maxStats = runeDataNew.mythicMaximumPossible;
                break;
            default:
                minStats = runeDataNew.commonMinimumPossible;
                maxStats = runeDataNew.commonMaximumPossible;
                break;
        }

        // Generate stats within the min and max range
        RuneDataNew.Stats rolledStats = new RuneDataNew.Stats
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

        return rolledStats;
    }
}
