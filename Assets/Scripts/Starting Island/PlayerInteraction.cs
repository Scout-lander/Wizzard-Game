using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Chest chest; // Reference to the chest script
    public WeaponInventory weaponInventory; // Reference to the weapon inventory
    public Portal portal; // Reference to the portal script
    public TMP_Text weaponNameText; // Reference to the TMP text for displaying character name
    public TMP_Text weaponDescriptionText;

    private int currentCharacterIndex = 0;

    private void Start()
    {
        UpdateCharacterNameText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Cycle to the next character
        {
            CycleNextCharacter();
        }

        if (Input.GetKeyDown(KeyCode.Q)) // Cycle to the previous character
        {
            CyclePreviousCharacter();
        }

        if (Input.GetKeyDown(KeyCode.F)) // Assuming 'F' is the key to interact with the portal
        {
            // Assuming the player interacts with the portal to transition to the game scene
            portal.OnTriggerEnter2D(GetComponent<Collider2D>());
        }
    }

    private void CycleNextCharacter()
    {
        currentCharacterIndex++;
        if (currentCharacterIndex >= weaponInventory.characterSlots.Length)
        {
            currentCharacterIndex = 0;
        }
        EquipCurrentCharacter();
    }

    private void CyclePreviousCharacter()
    {
        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
        {
            currentCharacterIndex = weaponInventory.characterSlots.Length - 1;
        }
        EquipCurrentCharacter();
    }

    private void EquipCurrentCharacter()
    {
        if (weaponInventory.characterSlots[currentCharacterIndex] != null)
        {
            weaponInventory.EquipCharacter(currentCharacterIndex);
            UpdateCharacterNameText();
            //Debug.Log($"Character {currentCharacterIndex} equipped");
        }
    }

    private void UpdateCharacterNameText()
    {
        if (weaponInventory.equippedCharacter != null)
        {
            weaponNameText.text = "Equipped: " + weaponInventory.equippedCharacter.StartingWeapon.weaponName;
            weaponDescriptionText.text = "Description: " + weaponInventory.equippedCharacter.StartingWeapon.weaponDescription;
        }
        else
        {
            weaponNameText.text = "Equipped: None";
        }
    }

    public void SelectCharacterFromSlot(int slotIndex)
    {
        //Debug.Log($"SelectCharacterFromSlot called with index {slotIndex}");
        if (slotIndex >= 0 && slotIndex < weaponInventory.characterSlots.Length)
        {
            currentCharacterIndex = slotIndex;
            EquipCurrentCharacter();
        }
    }
}
