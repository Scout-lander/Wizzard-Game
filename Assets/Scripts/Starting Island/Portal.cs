using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string targetSceneName = "GameScene"; // Name of the scene to transition to
    public WeaponInventory weaponInventory; // Reference to the player's weapon inventory

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Pass the equipped character to the CharacterSelector
            if (weaponInventory.equippedCharacter != null)
            {
                CharacterSelector.instance.SetEquippedCharacter(weaponInventory.equippedCharacter);
            }

            // Load the target scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
