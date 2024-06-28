using UnityEngine;

[CreateAssetMenu(fileName = "Luck Gem", menuName = "Inventory/Gem/LuckGem")]
public class LuckGem : GemData
{
    public float minLuckIncrease;
    public float maxLuckIncrease;
    [HideInInspector]
    public float luckIncrease;

    public float minCurseIncrease;
    public float maxCurseIncrease;
    [HideInInspector]
    public float curseIncrease;

    public override void InitializeRandomValues()
    {
        if (!isInitialized)
        {
            luckIncrease = Random.Range(minLuckIncrease, maxLuckIncrease);
            curseIncrease = Random.Range(minCurseIncrease, maxCurseIncrease);
            isInitialized = true;
        }
    }
}
