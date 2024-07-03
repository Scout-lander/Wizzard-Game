using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Speed Gem", menuName = "Inventory/Gem/SpeedGem")]
public class SpeedGem : GemData
{
    [Header("Common")]
    public float cMinMoveSpeedIncrease = 0.01f;
    public float cMaxMoveSpeedIncrease = 0.05f;
    public float cMinHealthDecrease = 1.0f;
    public float cMaxHealthDecrease = 5.0f;

    [Header("UnCommon")]
    public float uMinMoveSpeedIncrease = 0.05f;
    public float uMaxMoveSpeedIncrease = 0.1f;
    public float uMinHealthDecrease = 5.0f;
    public float uMaxHealthDecrease = 10.0f;

    [Header("Rare")]
    public float rMinMoveSpeedIncrease = 0.1f;
    public float rMaxMoveSpeedIncrease = 0.2f;
    public float rMinHealthDecrease = 10.0f;
    public float rMaxHealthDecrease = 15.0f;

    [Header("Epic")]
    public float eMinMoveSpeedIncrease = 0.2f;
    public float eMaxMoveSpeedIncrease = 0.3f;
    public float eMinHealthDecrease = 15.0f;
    public float eMaxHealthDecrease = 20.0f;

    [Header("Legendary")]
    public float lMinMoveSpeedIncrease = 0.3f;
    public float lMaxMoveSpeedIncrease = 0.4f;
    public float lMinHealthDecrease = 20.0f;
    public float lMaxHealthDecrease = 25.0f;

    [Header("Mithic")]
    public float mMinMoveSpeedIncrease = 0.4f;
    public float mMaxMoveSpeedIncrease = 0.5f;
    public float mMinHealthDecrease = 0.0f;
    public float mMaxHealthDecrease = 0.0f;

    private float minMoveSpeedIncrease;
    private float maxMoveSpeedIncrease;
    private float minHealthDecrease;
    private float maxHealthDecrease;

    [Header("OutCome")]
    public float moveSpeedIncrease;
    public float healthDecrease;

    public override void InitializeRandomValues()
    {
        if (!isInitialized)
        {
            AssignRandomRarity();
            SetStatRangesBasedOnRarity();
            moveSpeedIncrease = Random.Range(minMoveSpeedIncrease, maxMoveSpeedIncrease);
            healthDecrease = Random.Range(minHealthDecrease, maxHealthDecrease);
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
            legendaryProbabilities,
            mithicProbabilities
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
                minMoveSpeedIncrease = cMinMoveSpeedIncrease;
                maxMoveSpeedIncrease = cMaxMoveSpeedIncrease;
                minHealthDecrease = cMinHealthDecrease;
                maxHealthDecrease = cMaxHealthDecrease;
                break;
            case GemRarity.Uncommon:
                minMoveSpeedIncrease = uMinMoveSpeedIncrease;
                maxMoveSpeedIncrease = uMaxMoveSpeedIncrease;
                minHealthDecrease = uMinHealthDecrease;
                maxHealthDecrease = uMaxHealthDecrease;
                break;
            case GemRarity.Rare:
                minMoveSpeedIncrease = rMinMoveSpeedIncrease;
                maxMoveSpeedIncrease = rMaxMoveSpeedIncrease;
                minHealthDecrease = rMinHealthDecrease;
                maxHealthDecrease = rMaxHealthDecrease;
                break;
            case GemRarity.Epic:
                minMoveSpeedIncrease = eMinMoveSpeedIncrease;
                maxMoveSpeedIncrease = eMaxMoveSpeedIncrease;
                minHealthDecrease = eMinHealthDecrease;
                maxHealthDecrease = eMaxHealthDecrease;
                break;
            case GemRarity.Legendary:
                minMoveSpeedIncrease = lMinMoveSpeedIncrease;
                maxMoveSpeedIncrease = lMaxMoveSpeedIncrease;
                minHealthDecrease = lMinHealthDecrease;
                maxHealthDecrease = lMaxHealthDecrease;
                break;
            case GemRarity.Mithic:
                minMoveSpeedIncrease = mMinMoveSpeedIncrease;
                maxMoveSpeedIncrease = mMaxMoveSpeedIncrease;
                minHealthDecrease = mMinHealthDecrease;
                maxHealthDecrease = mMaxHealthDecrease;
                break;
            default:
                minMoveSpeedIncrease = cMinMoveSpeedIncrease;
                maxMoveSpeedIncrease = cMaxMoveSpeedIncrease;
                minHealthDecrease = cMinHealthDecrease;
                maxHealthDecrease = cMaxHealthDecrease;
                break;
        }
    }

#if UNITY_EDITOR
    public override GemData Clone(string path)
    {
        SpeedGem clone = Instantiate(this);
        clone.isInitialized = false; // Reset initialization flag for the clone
        clone.InitializeRandomValues();
        AssetDatabase.CreateAsset(clone, path);
        AssetDatabase.SaveAssets();
        return clone;
    }
#endif
}
