using System.Collections.Generic;
using UnityEngine;

public class RuneEffectApplier : MonoBehaviour
{
    public PlayerStats playerStats; // Reference to the PlayerStats script
    public RuneInventory runeInventory; // Reference to the RuneInventoryNew script

    void Start()
    {
        // Ensure the playerStats reference is assigned
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        // Apply the rune effects at the start
        ApplyEquippedRuneEffects();
    }

    public void ApplyEquippedRuneEffects()
    {
        // Reset the player's stats to base stats before applying runes
        playerStats.ActualStats = playerStats.baseStats;

        // Apply each rune's effects
        foreach (Rune rune in runeInventory.equippedRuneBag.runes)
        {
            ApplyRuneEffect(rune);
        }

        // Ensure the player's current health does not exceed the new max health
        if (playerStats.CurrentHealth > playerStats.ActualStats.maxHealth)
        {
            playerStats.CurrentHealth = playerStats.ActualStats.maxHealth;
        }

        // Update the health bar to reflect new max health
        playerStats.UpdateHealthBar();
    }

    public void ApplyRuneEffect(Rune rune)
    {
        var stats = rune.actualStats;

        playerStats.actualStats.maxHealth += stats.health;
        playerStats.actualStats.recovery += stats.lifeRegen;
        playerStats.actualStats.might += stats.might;
        playerStats.actualStats.moveSpeed += stats.moveSpeed;
        playerStats.actualStats.armor += stats.armor;
        playerStats.actualStats.speed += stats.attackSpeed;
        playerStats.actualStats.luck += stats.luck;
        playerStats.actualStats.curse += stats.curse;
        playerStats.actualStats.maxDashes += stats.dashCount;
        playerStats.actualStats.dashCooldown -= stats.dashCooldown;
    }
}
