using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    public GameObject characterInventoryUI; // Reference to the Character Inventory UI
    public GameObject WeaponSelection; // Reference to the Weapon Selection UI
    public SpriteRenderer chestSpriteRenderer; // Reference to the chest's sprite renderer
    public Sprite closedChestSprite; // Sprite for the closed chest
    public Sprite openChestSprite; // Sprite for the open chest

    private bool isPlayerNearby = false; // Track if player is nearby
    private ScreenManager screenManager; // Reference to the ScreenManager

    private void Start() // Ensure 'Start' is capitalized
    {
        WeaponSelection.SetActive(false); // Initialize Weapon Selection UI as inactive
        chestSpriteRenderer.sprite = closedChestSprite; // Set the chest to closed sprite initially
        screenManager = FindObjectOfType<ScreenManager>(); // Find the ScreenManager in the scene
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // Player is now nearby
            OpenInventoryUI();
            screenManager.OpenWeaponsScreen();
            chestSpriteRenderer.sprite = openChestSprite; // Change to open chest sprite
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // Player is no longer nearby
            screenManager.CloseCurrentScreen();
            chestSpriteRenderer.sprite = closedChestSprite; // Change back to closed chest sprite
        }
    }

    private void OpenInventoryUI()
    {
        characterInventoryUI.SetActive(true);
        WeaponSelection.SetActive(true);
        characterInventoryUI.GetComponent<CharacterInventoryUI>().UpdateUI();
    }

    private void CloseInventoryUI()
    {
        characterInventoryUI.SetActive(false);
        WeaponSelection.SetActive(false);
    }
}
