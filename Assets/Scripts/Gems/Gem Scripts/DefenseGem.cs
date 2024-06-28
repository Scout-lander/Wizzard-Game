using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Defense Gem", menuName = "Inventory/Gem/DefenseGem")]
public class DefenseGem : GemData
{
    [Header("Common")]
    public float cMaxArmorIncrease = 0.3f;
    public float cMinArmorIncrease = 0.1f;
    public float cMaxAttackSpeedDecrease = 1f;
    public float cMinAttackSpeedDecrease = 0.1f;
    public float commonProbabilities = 0.5f;

    [Header("UnCommon")]
    public float uMaxArmorIncrease = 0.5f;
    public float uMinArmorIncrease = 0.3f;
    public float uMaxAttackSpeedDecrease = 0.15f;
    public float uMinAttackSpeedDecrease = 0.1f;
    public float uncommonProbabilities = 0.3f;

    [Header("Rare")]
    public float rMaxArmorIncrease = 1.0f;
    public float rMinArmorIncrease = 0.5f;
    public float rMaxAttackSpeedDecrease = 0.2f;
    public float rMinAttackSpeedDecrease = 0.15f;
    public float rareProbabilities = 0.1f;

    [Header("Epic")]
    public float eMaxArmorIncrease = 1.5f;
    public float eMinArmorIncrease = 1.0f;
    public float eMaxAttackSpeedDecrease = 0.25f;
    public float eMinAttackSpeedDecrease = 0.2f;
    public float epicProbabilities = 0.07f;

    [Header("Legendary")]
    public float lMaxArmorIncrease = 2.0f;
    public float lMinArmorIncrease = 1.5f;
    public float lMaxAttackSpeedDecrease = 0.3f;
    public float lMinAttackSpeedDecrease = 0.25f;
    public float legendaryProbabilities = 0.03f;

    [Header("OutCome")]
    private float minArmorIncrease;
    private float maxArmorIncrease;
    private float minAttackSpeedDecrease;
    private float maxAttackSpeedDecrease;
    [HideInInspector]
    public float armorIncrease;
    [HideInInspector]
    public float attackSpeedDecrease;

    public override void InitializeRandomValues()
    {
        if (!isInitialized)
        {
            AssignRandomRarity();
            SetValuesBasedOnRarity();
            armorIncrease = Random.Range(minArmorIncrease, maxArmorIncrease);
            attackSpeedDecrease = Random.Range(minAttackSpeedDecrease, maxAttackSpeedDecrease);
            isInitialized = true;
        }
    }

    private void AssignRandomRarity()
    {
        float randomValue = Random.value;
        float cumulativeProbability = 0f;

        float[] rarityProbabilities = {
            commonProbabilities,
            uncommonProbabilities,
            rareProbabilities,
            epicProbabilities,
            legendaryProbabilities
        };

        for (int i = 0; i < rarityProbabilities.Length; i++)
        {
            cumulativeProbability += rarityProbabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                rarity = (GemRarity)i;
                return;
            }
        }

        rarity = GemRarity.Common; // Fallback in case something goes wrong
    }

    private void SetValuesBasedOnRarity()
    {
        switch (rarity)
        {
            case GemRarity.Common:
                minArmorIncrease = cMinArmorIncrease;
                maxArmorIncrease = cMaxArmorIncrease;
                minAttackSpeedDecrease = cMinAttackSpeedDecrease;
                maxAttackSpeedDecrease = cMaxAttackSpeedDecrease;
                break;
            case GemRarity.Uncommon:
                minArmorIncrease = uMinArmorIncrease;
                maxArmorIncrease = uMaxArmorIncrease;
                minAttackSpeedDecrease = uMinAttackSpeedDecrease;
                maxAttackSpeedDecrease = uMaxAttackSpeedDecrease;
                break;
            case GemRarity.Rare:
                minArmorIncrease = rMinArmorIncrease;
                maxArmorIncrease = rMaxArmorIncrease;
                minAttackSpeedDecrease = rMinAttackSpeedDecrease;
                maxAttackSpeedDecrease = rMaxAttackSpeedDecrease;
                break;
            case GemRarity.Epic:
                minArmorIncrease = eMinArmorIncrease;
                maxArmorIncrease = eMaxArmorIncrease;
                minAttackSpeedDecrease = eMinAttackSpeedDecrease;
                maxAttackSpeedDecrease = eMaxAttackSpeedDecrease;
                break;
            case GemRarity.Legendary:
                minArmorIncrease = lMinArmorIncrease;
                maxArmorIncrease = lMaxArmorIncrease;
                minAttackSpeedDecrease = lMinAttackSpeedDecrease;
                maxAttackSpeedDecrease = lMaxAttackSpeedDecrease;
                break;
            default:
                minArmorIncrease = cMinArmorIncrease;
                maxArmorIncrease = cMaxArmorIncrease;
                minAttackSpeedDecrease = cMinAttackSpeedDecrease;
                maxAttackSpeedDecrease = cMaxAttackSpeedDecrease;
                break;
        }
    }
}
