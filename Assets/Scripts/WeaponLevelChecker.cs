using UnityEngine;

public class WeaponLevelChecker : MonoBehaviour
{
    public PlayerInventory playerInventory;

    void Update()
    {
        // Check for input to trigger the weapon level check
        if (Input.GetKeyDown(KeyCode.T))
        {
            CheckWeaponLevels();
        }
    }

    void CheckWeaponLevels()
    {
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory reference not set in WeaponLevelChecker!");
            return;
        }

        foreach (PlayerInventory.Slot slot in playerInventory.weaponSlots)
        {
            if (slot.item is Weapon weapon)
            {
                Debug.Log($"Weapon: {weapon.name}, Level: {weapon.currentLevel}");
            }
        }
    }
}
