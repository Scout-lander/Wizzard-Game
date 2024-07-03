using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Health Gem", menuName = "Inventory/Gem/HealthGem")]
public class HealthGem : GemData
{
    [Header("Common")]
    public float cMaxGemHealth = 20;
    public float cMinGemHealth = 10;

    [Header("UnCommon")]
    public float uMaxGemHealth = 30;
    public float uMinGemHealth = 20;

    [Header("Rare")]
    public float rMaxGemHealth = 40;
    public float rMinGemHealth = 30;

    [Header("Epic")]
    public float eMaxGemHealth = 50;
    public float eMinGemHealth = 40;

    [Header("Legendary")]
    public float lMaxGemHealth = 60;
    public float lMinGemHealth = 50;

    [Header("Mithic")]
    public float mMaxGemHealth = 80;
    public float mMinGemHealth = 100;

    private float minGemHealth;
    private float maxGemHealth;
    
    [Header("OutCome")]
    public float currentGemHealth;

    public override void InitializeRandomValues()
    {
        if (!isInitialized)
        {
            AssignRandomRarity();
            SetHealthRangesBasedOnRarity();
            currentGemHealth = Random.Range(minGemHealth, maxGemHealth);
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

    private void SetHealthRangesBasedOnRarity()
    {
        switch (rarity)
        {
            case GemRarity.Common:
                minGemHealth = cMinGemHealth;
                maxGemHealth = cMaxGemHealth;
                break;
            case GemRarity.Uncommon:
                minGemHealth = uMinGemHealth;
                maxGemHealth = uMaxGemHealth;
                break;
            case GemRarity.Rare:
                minGemHealth = rMinGemHealth;
                maxGemHealth = rMaxGemHealth;
                break;
            case GemRarity.Epic:
                minGemHealth = eMinGemHealth;
                maxGemHealth = eMaxGemHealth;
                break;
            case GemRarity.Legendary:
                minGemHealth = lMinGemHealth;
                maxGemHealth = lMaxGemHealth;
                break;
            case GemRarity.Mythic:
                minGemHealth = mMinGemHealth;
                maxGemHealth = mMaxGemHealth;
                break;
            default:
                minGemHealth = cMinGemHealth;
                maxGemHealth = cMaxGemHealth;
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        currentGemHealth -= damage;
        if (currentGemHealth <= 0)
        {
            DestroyGem();
        }
    }

    private void DestroyGem()
    {
        Debug.Log($"{gemName} has been destroyed.");
        // You might want to remove the gem from the player's gem inventory here
    }

#if UNITY_EDITOR
    public override GemData Clone(string path)
    {
        HealthGem clone = Instantiate(this);
        clone.isInitialized = false;
        clone.InitializeRandomValues();
        AssetDatabase.CreateAsset(clone, path);
        AssetDatabase.SaveAssets();
        return clone;
    }
#endif
}
