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
    public float commonProbabilities = 0.5f;

    [Header("UnCommon")]
    public float uMaxGemHealth = 30;
    public float uMinGemHealth = 20;
    public float uncommonProbabilities = 0.3f;

    [Header("Rare")]
    public float rMaxGemHealth = 40;
    public float rMinGemHealth = 30;
    public float rareProbabilities = 0.1f;

    [Header("Epic")]
    public float eMaxGemHealth = 50;
    public float eMinGemHealth = 40;
    public float epicProbabilities = 0.07f;

    [Header("Legendary")]
    public float lMaxGemHealth = 60;
    public float lMinGemHealth = 50;
    public float legendaryProbabilities = 0.03f;

    [Header("OutCome")]
    private float minGemHealth;
    private float maxGemHealth;
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
