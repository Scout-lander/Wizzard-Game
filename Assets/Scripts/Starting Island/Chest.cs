using UnityEngine;

public class Chest : MonoBehaviour
{
    public CharacterData[] characterDataArray; // Array to hold character data
    public WeaponInventory weaponInventory; // Reference to the player's weapon inventory

    // Method to take a character from the chest
    public void TakeCharacter(int index)
    {
        if (index < 0 || index >= characterDataArray.Length)
        {
            Debug.LogError("Invalid character index.");
            return;
        }

        CharacterData selectedCharacter = characterDataArray[index];
        if (selectedCharacter == null)
        {
            Debug.LogError("Character data is null.");
            return;
        }

        // Add the character to the player's inventory
        bool added = weaponInventory.AddCharacter(selectedCharacter);
        if (added)
        {
            Debug.Log($"Character {selectedCharacter.Name} taken from chest.");
        }
    }
}
