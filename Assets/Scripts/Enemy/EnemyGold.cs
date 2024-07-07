using UnityEngine;

public class EnemyGold : MonoBehaviour
{
    public int goldAmount;

    // Call this method when the enemy dies
    public void DropGold()
    {
        // Add the gold amount to the GameManager's total gold collected
        if (GameManager.instance != null)
        {
            GameManager.instance.IncrementGold(goldAmount);
        }
        
        // Destroy the enemy game object
        Destroy(gameObject);
    }
}
