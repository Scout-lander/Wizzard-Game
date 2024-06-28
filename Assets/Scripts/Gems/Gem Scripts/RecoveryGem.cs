using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Recovery Gem", menuName = "Inventory/Gem/RecoveryGem")]
public class RecoveryGem : GemData
{
    [Header("Common")]
    public float cMinRecoveryIncrease = 0.01f;
    public float cMaxRecoveryIncrease = 0.05f;
    public float cMinMaxHealthDecrease = 1.0f;
    public float cMaxMaxHealthDecrease = 5.0f;
    public float commonProbabilities = 0.5f;

    [Header("UnCommon")]
    public float uMinRecoveryIncrease = 0.05f;
    public float uMaxRecoveryIncrease = 0.1f;
    public float uMinMaxHealthDecrease = 5.0f;
    public float uMaxMaxHealthDecrease = 10.0f;
    public float uncommonProbabilities = 0.3f;

    [Header("Rare")]
    public float rMinRecoveryIncrease = 0.1f;
    public float rMaxRecoveryIncrease = 0.2f;
    public float rMinMaxHealthDecrease = 10.0f;
    public float rMaxMaxHealthDecrease = 15.0f;
    public float rareProbabilities = 0.1f;

    [Header("Epic")]
    public float eMinRecoveryIncrease = 0.2f;
    public float eMaxRecoveryIncrease = 0.3f;
    public float eMinMaxHealthDecrease = 15.0f;
    public float eMaxMaxHealthDecrease = 20.0f;
    public float epicProbabilities = 0.07f;

    [Header("Legendary")]
    public float lMinRecoveryIncrease = 0.3f;
    public float lMaxRecoveryIncrease = 0.4f;
    public float lMinMaxHealthDecrease = 20.0f;
    public float lMaxMaxHealthDecrease = 25.0f;
    public float legendaryProbabilities = 0.03f;

    [Header("OutCome")]
    private float minRecoveryIncrease;
    private float maxRecoveryIncrease;
    private float minMaxHealthDecrease;
    private float maxMaxHealthDecrease;
    [HideInInspector]
    public float recoveryIncrease;
    [HideInInspector]
    public float maxHealthDecrease;

    public override void InitializeRandomValues()
    {
        if (!isInitialized)
        {
            AssignRandomRarity();
            SetStatRangesBasedOnRarity();
            recoveryIncrease = Random.Range(minRecoveryIncrease, maxRecoveryIncrease);
            maxHealthDecrease = Random.Range(minMaxHealthDecrease, maxMaxHealthDecrease);
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
                minRecoveryIncrease = cMinRecoveryIncrease;
                maxRecoveryIncrease = cMaxRecoveryIncrease;
                minMaxHealthDecrease = cMinMaxHealthDecrease;
                maxMaxHealthDecrease = cMaxMaxHealthDecrease;
                break;
            case GemRarity.Uncommon:
                minRecoveryIncrease = uMinRecoveryIncrease;
                maxRecoveryIncrease = uMaxRecoveryIncrease;
                minMaxHealthDecrease = uMinMaxHealthDecrease;
                maxMaxHealthDecrease = uMaxMaxHealthDecrease;
                break;
            case GemRarity.Rare:
                minRecoveryIncrease = rMinRecoveryIncrease;
                maxRecoveryIncrease = rMaxRecoveryIncrease;
                minMaxHealthDecrease = rMinMaxHealthDecrease;
                maxMaxHealthDecrease = rMaxMaxHealthDecrease;
                break;
            case GemRarity.Epic:
                minRecoveryIncrease = eMinRecoveryIncrease;
                maxRecoveryIncrease = eMaxRecoveryIncrease;
                minMaxHealthDecrease = eMinMaxHealthDecrease;
                maxMaxHealthDecrease = eMaxMaxHealthDecrease;
                break;
            case GemRarity.Legendary:
                minRecoveryIncrease = lMinRecoveryIncrease;
                maxRecoveryIncrease = lMaxRecoveryIncrease;
                minMaxHealthDecrease = lMinMaxHealthDecrease;
                maxMaxHealthDecrease = lMaxMaxHealthDecrease;
                break;
            default:
                minRecoveryIncrease = cMinRecoveryIncrease;
                maxRecoveryIncrease = cMaxRecoveryIncrease;
                minMaxHealthDecrease = cMinMaxHealthDecrease;
                maxMaxHealthDecrease = cMaxMaxHealthDecrease;
                break;
        }
    }

#if UNITY_EDITOR
    public override GemData Clone(string path)
    {
        RecoveryGem clone = Instantiate(this);
        clone.isInitialized = false; // Reset initialization flag for the clone
        clone.InitializeRandomValues();
        AssetDatabase.CreateAsset(clone, path);
        AssetDatabase.SaveAssets();
        return clone;
    }
#endif
}
