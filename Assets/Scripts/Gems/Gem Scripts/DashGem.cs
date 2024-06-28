using UnityEngine;

[CreateAssetMenu(fileName = "Dash Gem", menuName = "Inventory/Gem/DashGem")]
public class DashGem : GemData
{
    public float minDashCountIncrease;
    public float maxDashCountIncrease;
    [HideInInspector]
    public float dashCountIncrease;

    public float minDashCooldownIncrease;
    public float maxDashCooldownIncrease;
    [HideInInspector]
    public float dashCooldownIncrease;

    public override void InitializeRandomValues()
    {
        if (!isInitialized)
        {
            dashCountIncrease = Random.Range(minDashCountIncrease, maxDashCountIncrease);
            dashCooldownIncrease = Random.Range(minDashCooldownIncrease, maxDashCooldownIncrease);
            isInitialized = true;
        }
    }
}
