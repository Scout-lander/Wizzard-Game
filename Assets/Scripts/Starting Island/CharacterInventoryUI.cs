using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInventoryUI : MonoBehaviour
{
    public WeaponInventory weaponInventory; // Reference to the player's weapon inventory
    public GameObject slotPrefab; // Prefab for the character slots
    public Transform inventoryPanel; // Parent transform for the slots
    public PlayerInteraction playerInteraction; // Reference to the PlayerInteraction script

    private void Start()
    {
        // Subscribe to the inventory changed event
        weaponInventory.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDestroy()
    {
        // Unsubscribe from the inventory changed event to avoid memory leaks
        weaponInventory.OnInventoryChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        // Clear existing slots
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        // Create new slots
        for (int i = 0; i < weaponInventory.characterSlots.Length; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel);

            CharacterData character = weaponInventory.characterSlots[i];
            if (character != null)
            {
                // Set the weapon icon
                Image weaponIconImage = slot.transform.Find("WeaponIcon").GetComponent<Image>();
                if (character.StartingWeapon != null)
                {
                    weaponIconImage.sprite = character.StartingWeapon.icon;
                    weaponIconImage.color = Color.white; // Make sure the icon is visible

                    // Add a button listener to update the weapon details panel
                    Button button = slot.transform.Find("WeaponIcon").GetComponent<Button>();
                    if (button != null)
                    {
                        int slotIndex = i; // Capture the current slot index
                        button.onClick.AddListener(() => OnSlotClicked(slotIndex));
                        //Debug.Log($"Button listener added for slot {slotIndex}");
                    }
                }
                else
                {
                    weaponIconImage.color = Color.clear; // Set to transparent if no weapon
                }
            }
            else
            {
                // Set the weapon icon to be transparent if there is no character
                Image weaponIconImage = slot.transform.Find("WeaponIcon").GetComponent<Image>();
                weaponIconImage.color = Color.clear;
            }

            // Highlight the equipped character slot
            GameObject highlight = slot.transform.Find("Highlight").gameObject;
            if (weaponInventory.equippedCharacter == character)
            {
                highlight.SetActive(true);
            }
            else
            {
                highlight.SetActive(false);
            }
        }
    }

    private void OnSlotClicked(int slotIndex)
    {
        //Debug.Log($"Slot {slotIndex} clicked");
        // Call the method in PlayerInteraction to select the character from the slot
        playerInteraction.SelectCharacterFromSlot(slotIndex);
    }
}
