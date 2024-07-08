using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;

    public WeaponData weaponData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadWeaponData();
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }
    }

    private void LoadWeaponData()
    {
        string weaponName = PlayerPrefs.GetString("EquippedWeapon", "");
        if (!string.IsNullOrEmpty(weaponName))
        {
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in allAssetPaths)
            {
                if (assetPath.EndsWith(".asset"))
                {
                    WeaponData weapon = AssetDatabase.LoadAssetAtPath<WeaponData>(assetPath);
                    if (weapon != null && weapon.weaponName == weaponName)
                    {
                        weaponData = weapon;
                        Debug.Log($"Loaded weapon: {weapon.weaponName}");
                        return;
                    }
                }
            }
        }
    }

    public static WeaponData GetData()
    {
        if (instance && instance.weaponData)
            return instance.weaponData;
        else
        {
            // Randomly pick a weapon if we are playing from the Editor.
#if UNITY_EDITOR
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            List<WeaponData> weapons = new List<WeaponData>();
            foreach (string assetPath in allAssetPaths)
            {
                if (assetPath.EndsWith(".asset"))
                {
                    WeaponData weaponData = AssetDatabase.LoadAssetAtPath<WeaponData>(assetPath);
                    if (weaponData != null)
                    {
                        weapons.Add(weaponData);
                    }
                }
            }

            // Pick a random weapon if we have found any weapons.
            if (weapons.Count > 0) return weapons[Random.Range(0, weapons.Count)];
#endif
        }
        return null;
    }

    public void SelectWeapon(WeaponData weapon)
    {
        weaponData = weapon;
        SaveWeaponData();
    }

    public void SetEquippedWeapon(WeaponData weapon)
    {
        weaponData = weapon;
        SaveWeaponData();
        Debug.Log($"Equipped weapon set to {weapon.weaponName}."); // Ensure 'weaponName' exists in WeaponData
    }

    private void SaveWeaponData()
    {
        if (weaponData != null)
        {
            PlayerPrefs.SetString("EquippedWeapon", weaponData.weaponName);
            PlayerPrefs.Save();
        }
    }

    // Destroys the character selector.
    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
