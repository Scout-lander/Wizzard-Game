using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInventoryUI : MonoBehaviour
{
    public WeaponInventory weaponInventory; // Reference to the player's weapon inventory
    public GameObject slotPrefab; // Prefab for the weapon slots
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
        for (int i = 0; i < weaponInventory.weaponSlots.Length; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel);

            WeaponData weapon = weaponInventory.weaponSlots[i];
            if (weapon != null)
            {
                // Set the weapon icon
                Image weaponIconImage = slot.transform.Find("WeaponIcon").GetComponent<Image>();
                if (weapon.icon != null)
                {
                    weaponIconImage.sprite = weapon.icon;
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
                // Set the weapon icon to be transparent if there is no weapon
                Image weaponIconImage = slot.transform.Find("WeaponIcon").GetComponent<Image>();
                weaponIconImage.color = Color.clear;
            }

            // Highlight the equipped weapon slot
            GameObject highlight = slot.transform.Find("Highlight").gameObject;
            if (weaponInventory.equippedWeapon == weapon)
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
        // Call the method in PlayerInteraction to select the weapon from the slot
        playerInteraction.SelectWeaponFromSlot(slotIndex);
    }
}
