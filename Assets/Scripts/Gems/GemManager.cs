using UnityEngine;

public class GemManager : MonoBehaviour
{
    public Gem equippedGem;

    public void EquipGem(Gem newGem)
    {
        equippedGem = newGem;
        UpdateStats();
    }

    void UpdateStats()
    {
        // Implement the logic to update player and enemy stats based on the equipped gem.
        // For example:
        // playerHealth += equippedGem.playerStatEffect;
        // enemyStrength += equippedGem.enemyStatEffect;
    }
}
