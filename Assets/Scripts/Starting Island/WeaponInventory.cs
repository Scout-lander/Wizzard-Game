using UnityEngine;
using System;

public class WeaponInventory : MonoBehaviour
{
    public WeaponData[] weaponSlots = new WeaponData[10]; // Array to hold weapon data slots
    public WeaponData equippedWeapon; // The currently equipped weapon data

    public event Action OnInventoryChanged; // Event to notify UI when inventory changes

    private void Start()
    {
        LoadEquippedWeapon();
    }

    private void LoadEquippedWeapon()
    {
        string equippedWeaponName = PlayerPrefs.GetString("EquippedWeapon", "");
        if (!string.IsNullOrEmpty(equippedWeaponName))
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i] != null && weaponSlots[i].weaponName == equippedWeaponName)
                {
                    equippedWeapon = weaponSlots[i];
                    Debug.Log($"Loaded equipped weapon: {equippedWeapon.weaponName}");
                    return;
                }
            }
        }
    }

    private void SaveEquippedWeapon()
    {
        if (equippedWeapon != null)
        {
            PlayerPrefs.SetString("EquippedWeapon", equippedWeapon.weaponName);
            PlayerPrefs.Save();
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
