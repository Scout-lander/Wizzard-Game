using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Power Gem", menuName = "Inventory/Gem/PowerGem")]
public class PowerGem : GemData
{
    [Header("Common")]
    public float cMinAttackPowerIncrease = 0.01f;
    public float cMaxAttackPowerIncrease = 0.05f;
    public float cMinMoveSpeedDecrease = 0.1f;
    public float cMaxMoveSpeedDecrease = 0.5f;
    public float commonProbabilities = 0.5f;

    [Header("UnCommon")]
    public float uMinAttackPowerIncrease = 0.02f;
    public float uMaxAttackPowerIncrease = 0.1f;
    public float uMinMoveSpeedDecrease = 0.2f;
    public float uMaxMoveSpeedDecrease = 0.5f;
    public float uncommonProbabilities = 0.3f;

    [Header("Rare")]
    public float rMinAttackPowerIncrease = 0.03f;
    public float rMaxAttackPowerIncrease = 0.2f;
    public float rMinMoveSpeedDecrease = 0.3f;
    public float rMaxMoveSpeedDecrease = 0.5f;
    public float rareProbabilities = 0.1f;

    [Header("Epic")]
    public float eMinAttackPowerIncrease = 0.04f;
    public float eMaxAttackPowerIncrease = 0.3f;
    public float eMinMoveSpeedDecrease = 0.4f;
    public float eMaxMoveSpeedDecrease = 0.5f;
    public float epicProbabilities = 0.07f;

    [Header("Legendary")]
    public float lMinAttackPowerIncrease = 0.05f;
    public float lMaxAttackPowerIncrease = 1.0f;
    public float lMinMoveSpeedDecrease = 0.01f;
    public float lMaxMoveSpeedDecrease = 0.4f;
    public float legendaryProbabilities = 0.03f;

    [Header("OutCome")]
    private float minAttackPowerIncrease;
    private float maxAttackPowerIncrease;
    private float minMoveSpeedDecrease;
    private float maxMoveSpeedDecrease;
    [HideInInspector]
    public float attackPowerIncrease;
    [HideInInspector]
    public float moveSpeedDecrease;

    public override void InitializeRandomValues()
    {
        if (!isInitialized)
        {
            AssignRandomRarity();
            SetStatRangesBasedOnRarity();
            attackPowerIncrease = Random.Range(minAttackPowerIncrease, maxAttackPowerIncrease);
            moveSpeedDecrease = Random.Range(minMoveSpeedDecrease, maxMoveSpeedDecrease);
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

    private void SetStatRangesBasedOnRarity()
    {
        switch (rarity)
        {
            case GemRarity.Common:
                minAttackPowerIncrease = cMinAttackPowerIncrease;
                maxAttackPowerIncrease = cMaxAttackPowerIncrease;
                minMoveSpeedDecrease = cMinMoveSpeedDecrease;
                maxMoveSpeedDecrease = cMaxMoveSpeedDecrease;
                break;
            case GemRarity.Uncommon:
                minAttackPowerIncrease = uMinAttackPowerIncrease;
                maxAttackPowerIncrease = uMaxAttackPowerIncrease;
                minMoveSpeedDecrease = uMinMoveSpeedDecrease;
                maxMoveSpeedDecrease = uMaxMoveSpeedDecrease;
                break;
            case GemRarity.Rare:
                minAttackPowerIncrease = rMinAttackPowerIncrease;
                maxAttackPowerIncrease = rMaxAttackPowerIncrease;
                minMoveSpeedDecrease = rMinMoveSpeedDecrease;
                maxMoveSpeedDecrease = rMaxMoveSpeedDecrease;
                break;
            case GemRarity.Epic:
                minAttackPowerIncrease = eMinAttackPowerIncrease;
                maxAttackPowerIncrease = eMaxAttackPowerIncrease;
                minMoveSpeedDecrease = eMinMoveSpeedDecrease;
                maxMoveSpeedDecrease = eMaxMoveSpeedDecrease;
                break;
            case GemRarity.Legendary:
                minAttackPowerIncrease = lMinAttackPowerIncrease;
                maxAttackPowerIncrease = lMaxAttackPowerIncrease;
                minMoveSpeedDecrease = lMinMoveSpeedDecrease;
                maxMoveSpeedDecrease = lMaxMoveSpeedDecrease;
                break;
            default:
                minAttackPowerIncrease = cMinAttackPowerIncrease;
                maxAttackPowerIncrease = cMaxAttackPowerIncrease;
                minMoveSpeedDecrease = cMinMoveSpeedDecrease;
                maxMoveSpeedDecrease = cMaxMoveSpeedDecrease;
                break;
        }
    }

#if UNITY_EDITOR
    public override GemData Clone(string path)
    {
        PowerGem clone = Instantiate(this);
        clone.isInitialized = false; // Reset initialization flag for the clone
        clone.InitializeRandomValues();
        AssetDatabase.CreateAsset(clone, path);
        AssetDatabase.SaveAssets();
        return clone;
    }
#endif
}
