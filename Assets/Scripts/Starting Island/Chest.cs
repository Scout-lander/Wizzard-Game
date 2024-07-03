using UnityEngine;

public class Chest : MonoBehaviour
{
    public WeaponData[] weaponDataArray; // Array to hold weapon data
    public WeaponInventory weaponInventory; // Reference to the player's weapon inventory

    // Method to take a weapon from the chest
    public void TakeWeapon(int index)
    {
        if (index < 0 || index >= weaponDataArray.Length)
        {
            Debug.LogError("Invalid weapon index.");
            return;
        }

        WeaponData selectedWeapon = weaponDataArray[index];
        if (selectedWeapon == null)
        {
            Debug.LogError("Weapon data is null.");
            return;
        }

        // Add the weapon to the player's inventory
        bool added = weaponInventory.AddWeapon(selectedWeapon);
        if (added)
        {
            Debug.Log($"Weapon {selectedWeapon.weaponName} taken from chest.");
        }
    }
}
