using UnityEngine;

public class EnemyGold : MonoBehaviour
{
    public int goldAmount;

    // Call this method when the enemy dies
    public void DropGold()
    {
        PlayerGold.Instance.AddGold(goldAmount);
        Destroy(gameObject);
    }
}
