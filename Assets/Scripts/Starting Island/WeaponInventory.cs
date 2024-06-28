using UnityEngine;
using System;

public class WeaponInventory : MonoBehaviour
{
    public CharacterData[] characterSlots = new CharacterData[10]; // Array to hold character data slots
    public CharacterData equippedCharacter; // The currently equipped character data

    public event Action OnInventoryChanged; // Event to notify UI when inventory changes

    // Method to add a character to the inventory
    public bool AddCharacter(CharacterData character)
    {
        for (int i = 0; i < characterSlots.Length; i++)
        {
            if (characterSlots[i] == null)
            {
                characterSlots[i] = character;
                Debug.Log($"Character {character.Name} added to slot {i}.");
                OnInventoryChanged?.Invoke(); // Notify UI about the change
                return true;
            }
        }
        Debug.Log("Inventory is full.");
        return false;
    }

    // Method to equip a character from the inventory
    public void EquipCharacter(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= characterSlots.Length || characterSlots[slotIndex] == null)
        {
            Debug.LogError("Invalid character slot index.");
            return;
        }

        equippedCharacter = characterSlots[slotIndex];
        Debug.Log($"Character {equippedCharacter.Name} equipped from slot {slotIndex}.");
        OnInventoryChanged?.Invoke(); // Notify UI about the change
    }

    // Method to remove a character from the inventory
    public void RemoveCharacter(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= characterSlots.Length || characterSlots[slotIndex] == null)
        {
            Debug.LogError("Invalid character slot index.");
            return;
        }

        Debug.Log($"Character {characterSlots[slotIndex].Name} removed from slot {slotIndex}.");
        characterSlots[slotIndex] = null;
        OnInventoryChanged?.Invoke(); // Notify UI about the change
    }
}
