using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Chest chest; // Reference to the chest script
    public WeaponInventory weaponInventory; // Reference to the weapon inventory
    public Portal portal; // Reference to the portal script
    public TMP_Text weaponNameText; // Reference to the TMP text for displaying weapon name
    public TMP_Text weaponDescriptionText; // Reference to the TMP text for displaying weapon description
    public GameObject weaponSelectorUI; // Reference to the weapon selector UI

    private int currentWeaponIndex = 0;

    private void Start()
    {
        UpdateWeaponNameText();
    }

    private void Update()
    {
        if (weaponSelectorUI.activeSelf)
        {
            //Time.timeScale = 0;

           if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // Cycle to the next weapon
            {
                CycleNextWeapon();
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // Cycle to the previous weapon
            {
                CyclePreviousWeapon();
            }
        }
        else
        {
            Time.timeScale = 1;


            if (Input.GetKeyDown(KeyCode.F)) // Assuming 'F' is the key to interact with the portal
            {
                // Assuming the player interacts with the portal to transition to the game scene
                portal.OnTriggerEnter2D(GetComponent<Collider2D>());
            }
        }
    }

    private void CycleNextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= weaponInventory.weaponSlots.Length)
        {
            currentWeaponIndex = 0;
        }
        EquipCurrentWeapon();
    }

    private void CyclePreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weaponInventory.weaponSlots.Length - 1;
        }
        EquipCurrentWeapon();
    }

    private void EquipCurrentWeapon()
    {
        if (weaponInventory.weaponSlots[currentWeaponIndex] != null)
        {
            weaponInventory.EquipWeapon(currentWeaponIndex);
            UpdateWeaponNameText();

            // Change the starting weapon in PlayerStats
            PlayerStats playerStats = GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.ChangeStartingWeapon(weaponInventory.weaponSlots[currentWeaponIndex]);
            }

            // Set the equipped weapon in CharacterSelector to persist across scenes
            CharacterSelector.instance.SetEquippedWeapon(weaponInventory.weaponSlots[currentWeaponIndex]);
        }
    }

    private void UpdateWeaponNameText()
    {
        if (weaponInventory.equippedWeapon != null)
        {
            weaponNameText.text = "Equipped: " + weaponInventory.equippedWeapon.weaponName;
            weaponDescriptionText.text = "Description: " + weaponInventory.equippedWeapon.weaponDescription;
        }
        else
        {
            weaponNameText.text = "Equipped: None";
            weaponDescriptionText.text = "Description: None";
        }
    }

    public void SelectWeaponFromSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < weaponInventory.weaponSlots.Length)
        {
            currentWeaponIndex = slotIndex;
            EquipCurrentWeapon();
        }
    }
}
