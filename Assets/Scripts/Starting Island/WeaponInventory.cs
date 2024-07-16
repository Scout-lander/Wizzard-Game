using UnityEngine;
using System;

public class WeaponInventory : MonoBehaviour
{
    public WeaponData[] weaponSlots = new WeaponData[10]; // Array to hold weapon data slots
    public WeaponData equippedWeapon; // The currently equipped weapon data

    public event Action OnInventoryChanged; // Event to notify UI when inventory changes

    private SaveLoadManager saveLoadManager;

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        LoadEquippedWeapon();
    }

    private void LoadEquippedWeapon()
    {
        equippedWeapon = saveLoadManager.LoadEquippedWeapon();
        if (equippedWeapon != null)
        {
            Debug.Log($"Loaded equipped weapon: {equippedWeapon.weaponName}");
        }
    }

    private void SaveEquippedWeapon()
    {
        if (equippedWeapon != null)
        {
            saveLoadManager.SaveEquippedWeapon(equippedWeapon);
        }
    }

    // Method to add a weapon to the inventory
    public bool AddWeapon(WeaponData weapon)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null)
            {
                weaponSlots[i] = weapon;
                Debug.Log($"Weapon {weapon.weaponName} added to slot {i}.");
                OnInventoryChanged?.Invoke(); // Notify UI about the change
                return true;
            }
        }
        Debug.Log("Inventory is full.");
        return false;
    }

    // Method to equip a weapon from the inventory
    public void EquipWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weaponSlots.Length || weaponSlots[slotIndex] == null)
        {
            Debug.LogError("Invalid weapon slot index.");
            return;
        }

        equippedWeapon = weaponSlots[slotIndex];
        SaveEquippedWeapon();
        Debug.Log($"Weapon {equippedWeapon.weaponName} equipped from slot {slotIndex}.");
        OnInventoryChanged?.Invoke(); // Notify UI about the change
    }

    // Method to remove a weapon from the inventory
    public void RemoveWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= weaponSlots.Length || weaponSlots[slotIndex] == null)
        {
            Debug.LogError("Invalid weapon slot index.");
            return;
        }

        Debug.Log($"Weapon {weaponSlots[slotIndex].weaponName} removed from slot {slotIndex}.");
        weaponSlots[slotIndex] = null;
        OnInventoryChanged?.Invoke(); // Notify UI about the change
    }
}
