using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Life Gem", menuName = "Inventory/Gem/LifeGem")]
public class LifeGem : GemData
{
    [Header("Common")]
    public float cMinLifeRegenPerSecond = 0.05f;
    public float cMaxLifeRegenPerSecond = 0.1f;
    public float cMinMaxHpIncrease = 1;
    public float cMaxMaxHpIncrease = 5;
    public float commonProbabilities = 0.5f;

    [Header("UnCommon")]
    public float uMinLifeRegenPerSecond = 0.1f;
    public float uMaxLifeRegenPerSecond = 0.2f;
    public float uMinMaxHpIncrease = 5;
    public float uMaxMaxHpIncrease = 10;
    public float uncommonProbabilities = 0.3f;

    [Header("Rare")]
    public float rMinLifeRegenPerSecond = 0.2f;
    public float rMaxLifeRegenPerSecond = 0.4f;
    public float rMinMaxHpIncrease = 10;
    public float rMaxMaxHpIncrease = 20;
    public float rareProbabilities = 0.1f;

    [Header("Epic")]
    public float eMinLifeRegenPerSecond = 0.4f;
    public float eMaxLifeRegenPerSecond = 0.6f;
    public float eMinMaxHpIncrease = 20;
    public float eMaxMaxHpIncrease = 30;
    public float epicProbabilities = 0.07f;

    [Header("Legendary")]
    public float lMinLifeRegenPerSecond = 0.6f;
    public float lMaxLifeRegenPerSecond = 1f;
    public float lMinMaxHpIncrease = 30;
    public float lMaxMaxHpIncrease = 40;
    public float legendaryProbabilities = 0.03f;

    [Header("OutCome")]
    private float minLifeRegenPerSecond;
    private float maxLifeRegenPerSecond;
    private float minMaxHpIncrease;
    private float maxMaxHpIncrease;
    [HideInInspector]
    public float lifeRegenPerSecond;
    [HideInInspector]
    public float maxHpIncrease;

    public override void InitializeRandomValues()
    {
        if (!isInitialized)
        {
            AssignRandomRarity();
            SetValuesBasedOnRarity();
            lifeRegenPerSecond = Random.Range(minLifeRegenPerSecond, maxLifeRegenPerSecond);
            maxHpIncrease = Random.Range(minMaxHpIncrease, maxMaxHpIncrease);
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
                minLifeRegenPerSecond = cMinLifeRegenPerSecond;
                maxLifeRegenPerSecond = cMaxLifeRegenPerSecond;
                minMaxHpIncrease = cMinMaxHpIncrease;
                maxMaxHpIncrease = cMaxMaxHpIncrease;
                break;
            case GemRarity.Uncommon:
                minLifeRegenPerSecond = uMinLifeRegenPerSecond;
                maxLifeRegenPerSecond = uMaxLifeRegenPerSecond;
                minMaxHpIncrease = uMinMaxHpIncrease;
                maxMaxHpIncrease = uMaxMaxHpIncrease;
                break;
            case GemRarity.Rare:
                minLifeRegenPerSecond = rMinLifeRegenPerSecond;
                maxLifeRegenPerSecond = rMaxLifeRegenPerSecond;
                minMaxHpIncrease = rMinMaxHpIncrease;
                maxMaxHpIncrease = rMaxMaxHpIncrease;
                break;
            case GemRarity.Epic:
                minLifeRegenPerSecond = eMinLifeRegenPerSecond;
                maxLifeRegenPerSecond = eMaxLifeRegenPerSecond;
                minMaxHpIncrease = eMinMaxHpIncrease;
                maxMaxHpIncrease = eMaxMaxHpIncrease;
                break;
            case GemRarity.Legendary:
                minLifeRegenPerSecond = lMinLifeRegenPerSecond;
                maxLifeRegenPerSecond = lMaxLifeRegenPerSecond;
                minMaxHpIncrease = lMinMaxHpIncrease;
                maxMaxHpIncrease = lMaxMaxHpIncrease;
                break;
            default:
                minLifeRegenPerSecond = cMinLifeRegenPerSecond;
                maxLifeRegenPerSecond = cMaxLifeRegenPerSecond;
                minMaxHpIncrease = cMinMaxHpIncrease;
                maxMaxHpIncrease = cMaxMaxHpIncrease;
                break;
        }
    }

#if UNITY_EDITOR
    public override GemData Clone(string path)
    {
        LifeGem clone = Instantiate(this);
        clone.isInitialized = false; // Reset initialization flag for the clone
        clone.InitializeRandomValues();
        AssetDatabase.CreateAsset(clone, path);
        AssetDatabase.SaveAssets();
        return clone;
    }
#endif
}
