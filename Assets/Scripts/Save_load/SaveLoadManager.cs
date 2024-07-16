using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    private string runeBagsSavePath;
    private string equippedWeaponSavePath;

    private void Awake()
    {
        runeBagsSavePath = Application.persistentDataPath + "/runeBags.json";
        equippedWeaponSavePath = Application.persistentDataPath + "/equippedWeapon.json";
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void SaveRuneBags(RuneBagSerializable runeBag, RuneBagSerializable equippedRuneBag)
    {
        RuneBagsData data = new RuneBagsData
        {
            runeBag = runeBag,
            equippedRuneBag = equippedRuneBag
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(runeBagsSavePath, json);
        Debug.Log("Rune bags saved to " + runeBagsSavePath);
    }

    public void LoadRuneBags(RuneInventory runeInventory)
    {
        if (File.Exists(runeBagsSavePath))
        {
            string json = File.ReadAllText(runeBagsSavePath);
            RuneBagsData data = JsonUtility.FromJson<RuneBagsData>(json);

            runeInventory.runeBag = data.runeBag;
            runeInventory.equippedRuneBag = data.equippedRuneBag;
            Debug.Log("Rune bags loaded from " + runeBagsSavePath);
        }
        else
        {
            Debug.LogWarning("Save file not found at " + runeBagsSavePath);
        }
    }

    public void SaveEquippedWeapon(WeaponData equippedWeapon)
    {
        EquippedWeaponData data = new EquippedWeaponData
        {
            equippedWeapon = equippedWeapon
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(equippedWeaponSavePath, json);
        Debug.Log("Equipped weapon saved to " + equippedWeaponSavePath);
    }

    public WeaponData LoadEquippedWeapon()
    {
        if (File.Exists(equippedWeaponSavePath))
        {
            string json = File.ReadAllText(equippedWeaponSavePath);
            EquippedWeaponData data = JsonUtility.FromJson<EquippedWeaponData>(json);
            Debug.Log("Equipped weapon loaded from " + equippedWeaponSavePath);
            return data.equippedWeapon;
        }
        else
        {
            Debug.LogWarning("Equipped weapon save file not found at " + equippedWeaponSavePath);
            return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RuneInventory runeInventory = FindObjectOfType<RuneInventory>();
        if (runeInventory != null)
        {
            LoadRuneBags(runeInventory);
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        RuneInventory runeInventory = FindObjectOfType<RuneInventory>();
        if (runeInventory != null)
        {
            SaveRuneBags(runeInventory.runeBag, runeInventory.equippedRuneBag);
        }
    }

    [System.Serializable]
    private class RuneBagsData
    {
        public RuneBagSerializable runeBag;
        public RuneBagSerializable equippedRuneBag;
    }

    [System.Serializable]
    private class EquippedWeaponData
    {
        public WeaponData equippedWeapon;
    }
}
