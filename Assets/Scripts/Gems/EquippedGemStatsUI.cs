using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EquippedGemStatsUI : MonoBehaviour
{
    public TMP_Text gemTotalsText; // Reference to the new Text component for displaying the total stats

    private void OnEnable()
    {
        UpdateEquippedGemStats(); // Update the equipped gem stats whenever the UI is enabled
    }

    public void UpdateEquippedGemStats()
    {
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        Dictionary<System.Type, GemStats> gemStatsDictionary = new Dictionary<System.Type, GemStats>();

        foreach (GemData gem in equippedBag.gems)
        {
            if (gem != null)
            {
                System.Type gemType = gem.GetType();
                if (!gemStatsDictionary.ContainsKey(gemType))
                {
                    gemStatsDictionary[gemType] = new GemStats();
                }
                gemStatsDictionary[gemType].AddStats(gem);
            }
        }

        string stats = "Equipped Gem Stats:\n";
        foreach (var entry in gemStatsDictionary)
        {
            stats += entry.Value.GetStatsSummary(entry.Key);
        }

        gemTotalsText.text = stats; // Update the new UI text component with the stats summary
    }

    private class GemStats
    {
        public float LifeRegen { get; private set; }
        public int MaxHpIncrease { get; private set; }
        public float AttackPowerIncrease { get; private set; }
        public float MoveSpeedDecrease { get; private set; }
        public float ArmorIncrease { get; private set; }
        public float AttackSpeedDecrease { get; private set; }
        public float MoveSpeedIncrease { get; private set; }
        public float HealthDecrease { get; private set; }
        public float LuckIncrease { get; private set; }
        public float CurseIncrease { get; private set; }
        public float RecoveryIncrease { get; private set; }
        public float DashCountIncrease { get; private set; }
        public float DashCooldownIncrease { get; private set; }
        public float GemsHp { get; private set; }

        public void AddStats(GemData gem)
        {
            if (gem is LifeGem lifeGem)
            {
                LifeRegen += lifeGem.lifeRegenPerSecond;
                MaxHpIncrease += (int)lifeGem.maxHpIncrease; // Cast to int
            }
            else if (gem is PowerGem powerGem)
            {
                AttackPowerIncrease += powerGem.attackPowerIncrease;
                MoveSpeedDecrease += powerGem.moveSpeedDecrease;
            }
            else if (gem is DefenseGem defenseGem)
            {
                ArmorIncrease += defenseGem.armorIncrease;
                AttackSpeedDecrease += defenseGem.attackSpeedDecrease;
            }
            else if (gem is SpeedGem speedGem)
            {
                MoveSpeedIncrease += speedGem.moveSpeedIncrease;
                HealthDecrease += speedGem.healthDecrease;
            }
            else if (gem is LuckGem luckGem)
            {
                LuckIncrease += luckGem.luckIncrease;
                CurseIncrease += luckGem.curseIncrease;
            }
            else if (gem is RecoveryGem recoveryGem)
            {
                RecoveryIncrease += recoveryGem.recoveryIncrease;
                MaxHpIncrease += (int)recoveryGem.maxHealthDecrease; // Cast to int
            }
            else if (gem is DashGem dashGem)
            {
                DashCountIncrease += dashGem.dashCountIncrease;
                DashCooldownIncrease += dashGem.dashCooldownIncrease;
            }
            else if (gem is HealthGem healthGem)
            {
                GemsHp += healthGem.currentGemHealth;
            }
        }

        public string GetStatsSummary(System.Type gemType)
        {
            string summary = "";
            if (gemType == typeof(LifeGem))
            {
                summary += $"<color=yellow>Life Regen</color>: {LifeRegen:F2}\n<color=yellow>Max HP Increase</color>: {MaxHpIncrease}\n";
            }
            else if (gemType == typeof(PowerGem))
            {
                summary += $"<color=yellow>Might Increase</color>: {AttackPowerIncrease * 100:F2}%\n<color=red>Move Speed Decrease</color>: -{MoveSpeedDecrease * 100:F2}%\n";
            }
            else if (gemType == typeof(DefenseGem))
            {
                summary += $"<color=yellow>Armor Increase</color>: {ArmorIncrease:F2}\n<color=red>Attack Speed Decrease</color>: -{AttackSpeedDecrease * 100:F2}%\n";
            }
            else if (gemType == typeof(SpeedGem))
            {
                summary += $"<color=yellow>Move Speed Increase</color>: {MoveSpeedIncrease * 100:F2}%\n<color=red>Max Health Decrease</color>: {HealthDecrease:F2}\n";
            }
            else if (gemType == typeof(LuckGem))
            {
                summary += $"<color=yellow>Luck Increase</color>: {LuckIncrease * 100:F2}%\n<color=red>Curse Increase</color>: {CurseIncrease * 100:F2}%\n";
            }
            else if (gemType == typeof(RecoveryGem))
            {
                summary += $"<color=yellow>Recovery Increase</color>: {RecoveryIncrease:F2}\n<color=red>Max Health Decrease</color>: {MaxHpIncrease}\n";
            }
            else if (gemType == typeof(DashGem))
            {
                summary += $"<color=yellow>Dash Count Increase</color>: {DashCountIncrease:F2}\n<color=red>Dash Cooldown Increase</color>: {DashCooldownIncrease:F2}\n";
            }
            else if (gemType == typeof(HealthGem))
            {
                summary += $"<color=yellow>Gems HP</color>: {GemsHp:F2}\n";
            }
            return summary;
        }
    }
}
