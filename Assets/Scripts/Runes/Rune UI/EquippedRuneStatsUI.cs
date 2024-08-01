using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedRuneStatsUI : MonoBehaviour
{
    public TMP_Text runeTotalsText;
    public ScrollRect scrollRect;

    private void OnEnable()
    {
        UpdateEquippedRuneStats();
    }

    public void UpdateEquippedRuneStats()
    {
        RuneInventory runeInventory = FindObjectOfType<RuneInventory>();
        if (runeInventory == null)
        {
            Debug.LogError("RuneInventory not found in the scene.");
            return;
        }

        RuneStats combinedStats = new RuneStats();

        foreach (Rune rune in runeInventory.equippedRuneBag.runes)
        {
            if (rune != null && rune.actualStats != null)
            {
                combinedStats.AddStats(rune.actualStats);
            }
        }

        string statsSummary = combinedStats.GetCombinedStatsSummary();
        runeTotalsText.text = statsSummary;

        // Force update the scroll rect to adjust its size
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1; // Scroll to top
    }

    private class RuneStats
    {
        public int Health { get; private set; }
        public float AttackSpeed { get; private set; }
        public float Luck { get; private set; }
        public float Curse { get; private set; }
        public float DashCount { get; private set; }
        public float DashCooldown { get; private set; }
        public float Armor { get; private set; }
        public float HeartRune { get; private set; }
        public float LifeRegen { get; private set; }
        public float Might { get; private set; }
        public float MoveSpeed { get; private set; }

        public void AddStats(RuneDataNew.Stats stats)
        {
            Health += stats.health;
            AttackSpeed += stats.attackSpeed;
            Luck += stats.luck;
            Curse += stats.curse;
            DashCount += stats.dashCount;
            DashCooldown += stats.dashCooldown;
            Armor += stats.armor;
            HeartRune += stats.heartRune;
            LifeRegen += stats.lifeRegen;
            Might += stats.might;
            MoveSpeed += stats.moveSpeed;
        }

        public string GetCombinedStatsSummary()
        {
            string summary = "Equipped Rune Stats:\n";

            if (Health > 0)
                summary += $"<color=yellow>Health</color>: {Health}\n";
                if (Health < 0)
                    summary += $"<color=red>Health</color>: {Health}\n";
            if (AttackSpeed > 0)
                summary += $"<color=yellow>Attack Speed</color>: {AttackSpeed:F2}\n";
                if (AttackSpeed < 0) 
                    summary += $"<color=red>Attack Speed</color>: {AttackSpeed:F2}\n";
            if (Luck > 0)
                summary += $"<color=yellow>Luck</color>: {Luck}\n";
                if (Luck < 0) 
                    summary += $"<color=red>Luck</color>: {Luck}\n";
            if (Curse > 0)
                summary += $"<color=red>Curse</color>: {Curse}\n";
                if (Curse < 0) 
                    summary += $"<color=red>Curse</color>: {Curse}\n";
            if (DashCount > 0)
                summary += $"<color=yellow>Dash Count</color>: {DashCount:F2}\n";
                if (DashCount < 0) 
                    summary += $"<color=red>Dash Count</color>: {DashCount:F2}\n";
            if (DashCooldown > 0)
                summary += $"<color=yellow>Dash Cooldown</color>: {DashCooldown:F2}\n";
                if (DashCooldown < 0) 
                    summary += $"<color=red>Dash Cooldown</color>: {DashCooldown:F2}\n";
            if (Armor > 0)
                summary += $"<color=yellow>Armor</color>: {Armor}\n";
                if (Armor < 0) 
                    summary += $"<color=red>Armor</color>: {Armor}\n";
            if (HeartRune > 0)
                summary += $"<color=yellow>Gem Health</color>: {HeartRune}\n";
            if (LifeRegen > 0)
                summary += $"<color=yellow>Life Regen</color>: {LifeRegen:F2}\n";
                if (LifeRegen < 0) 
                    summary += $"<color=red>Life Regen</color>: {LifeRegen:F2}\n";
            if (Might > 0)
                summary += $"<color=yellow>Might</color>: {Might:F2}\n";
                if (Might < 0) 
                    summary += $"<color=red>Might</color>: {Might:F2}\n";
            if (MoveSpeed > 0)
                summary += $"<color=yellow>Move Speed</color>: {MoveSpeed * 100:F2}%\n";
                if (MoveSpeed < 0) 
                    summary += $"<color=red>Move Speed</color>: {MoveSpeed * 100:F2}%\n";

            return summary;
        }
    }
}
