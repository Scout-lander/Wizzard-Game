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
            // Pass the equipped weapon to the CharacterSelector
            if (weaponInventory.equippedWeapon != null)
            {
                CharacterSelector.instance.SetEquippedWeapon(weaponInventory.equippedWeapon);
            }

            // Load the target scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
