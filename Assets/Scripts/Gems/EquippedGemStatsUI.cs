using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedGemStatsUI : MonoBehaviour
{
    public TMP_Text gemTotalsText;
    public ScrollRect scrollRect;

    private void OnEnable()
    {
        UpdateEquippedGemStats();
    }

    public void UpdateEquippedGemStats()
    {
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        GemStats combinedStats = new GemStats();

        foreach (GemData gem in equippedBag.gems)
        {
            if (gem != null)
            {
                combinedStats.AddStats(gem);
            }
        }

        string statsSummary = combinedStats.GetCombinedStatsSummary();
        gemTotalsText.text = statsSummary;

        // Force update the scroll rect to adjust its size
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1; // Scroll to top
    }

    private class GemStats
    {
        public float MaxHealth { get; private set; }
        public float MoveSpeed { get; private set; }
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
                MaxHealth += (int)lifeGem.maxHpIncrease;
            }
            else if (gem is PowerGem powerGem)
            {
                AttackPowerIncrease += powerGem.attackPowerIncrease;
                MoveSpeed -= powerGem.moveSpeedDecrease;
            }
            else if (gem is DefenseGem defenseGem)
            {
                ArmorIncrease += defenseGem.armorIncrease;
                AttackSpeedDecrease += defenseGem.attackSpeedDecrease;
            }
            else if (gem is SpeedGem speedGem)
            {
                MoveSpeed += speedGem.moveSpeedIncrease;
                MaxHealth -= speedGem.healthDecrease;
            }
            else if (gem is LuckGem luckGem)
            {
                LuckIncrease += luckGem.luckIncrease;
                CurseIncrease += luckGem.curseIncrease;
            }
            else if (gem is RecoveryGem recoveryGem)
            {
                LifeRegen += recoveryGem.recoveryIncrease;
                MaxHealth -= (int)recoveryGem.maxHealthDecrease;
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

        public string GetCombinedStatsSummary()
        {
            string summary = "Equipped Gem Stats:\n";

            if (MaxHealth > 0)
                summary += $"<color=yellow>Max Health</color>: {MaxHealth:F2}\n";
            if (MaxHealth < 0)
                summary += $"<color=red>Max Health</color>: {MaxHealth:F2}\n";
            if (MoveSpeed > 0)
                summary += $"<color=yellow>Move Speed</color>: {MoveSpeed * 100:F2}%\n";
            if (MoveSpeed < 0)
                summary += $"<color=red>Move Speed</color>: {MoveSpeed * 100:F2}%\n";
            if (LifeRegen > 0)
                summary += $"<color=yellow>Life Regen</color>: {LifeRegen:F2}\n";
            if (MaxHpIncrease != 0)
                summary += $"<color=yellow>Max HP Increase</color>: {MaxHpIncrease}\n";
            if (AttackPowerIncrease > 0)
                summary += $"<color=yellow>Might Increase</color>: {AttackPowerIncrease * 100:F2}%\n";
            if (MoveSpeedDecrease < 0)
                summary += $"<color=red>Move Speed Decrease</color>: -{MoveSpeedDecrease * 100:F2}%\n";
            if (ArmorIncrease > 0)
                summary += $"<color=yellow>Armor Increase</color>: {ArmorIncrease:F2}\n";
            if (AttackSpeedDecrease > 0)
                summary += $"<color=red>Attack Speed Decrease</color>: -{AttackSpeedDecrease * 100:F2}%\n";
            if (MoveSpeedIncrease > 0)
                summary += $"<color=yellow>Move Speed Increase</color>: {MoveSpeedIncrease * 100:F2}%\n";
            if (HealthDecrease > 0)
                summary += $"<color=red>Max Health Decrease</color>: {HealthDecrease:F2}\n";
            if (LuckIncrease > 0)
                summary += $"<color=yellow>Luck Increase</color>: {LuckIncrease * 100:F2}%\n";
            if (CurseIncrease > 0)
                summary += $"<color=red>Curse Increase</color>: {CurseIncrease * 100:F2}%\n";
            if (RecoveryIncrease > 0)
                summary += $"<color=yellow>Recovery Increase</color>: {RecoveryIncrease:F2}\n";
            if (DashCountIncrease > 0)
                summary += $"<color=yellow>Dash Count Increase</color>: {DashCountIncrease:F2}\n";
            if (DashCooldownIncrease > 0)
                summary += $"<color=red>Dash Cooldown Increase</color>: {DashCooldownIncrease:F2}\n";
            if (GemsHp > 0)
                summary += $"<color=yellow>Gems HP</color>: {GemsHp:F2}\n";

            return summary;
        }
    }
}
