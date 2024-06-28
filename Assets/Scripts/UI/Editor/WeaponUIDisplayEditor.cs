using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponUIDisplay))]
public class WeaponUIDisplayEditor : Editor
{

    WeaponUIDisplay uiDisplay;

    private void OnEnable()
    {
        uiDisplay = target as WeaponUIDisplay;
    }

    // This function draws the inspector.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //if (GUILayout.Button("Find All Available Weapons"))
        //{
        //    uiDisplay.allWeapons = FindAllWeaponDataAssets();
        //}

        // This has been updated to spawn as many weapon slots as you have weapons.
        if (GUILayout.Button("Spawn Weapon Slots"))
        {
            // Register the action as being undoable.
            Undo.RegisterCompleteObjectUndo(uiDisplay, "Spawn Weapon Slots");

            // Delete all existing slots first because we want to respawn them.
            for (int i = 1; i < uiDisplay.transform.childCount; i++)
            {
                Undo.DestroyObjectImmediate(uiDisplay.transform.GetChild(i).gameObject);
                i--;
            }
            uiDisplay.weaponSlots.Clear();
            if(uiDisplay.transform.childCount > 0)
            {
                uiDisplay.slotTemplate = uiDisplay.transform.GetChild(0).gameObject;
                uiDisplay.weaponSlots.Add(uiDisplay.slotTemplate.transform);
            }
            

            // Create a slot for each weapon data that we have.
            if (uiDisplay.slotTemplate)
            {
                int uniqueWeaponCount = FindAllWeaponDataAssets().Length;
                for (int i = 1; i < uniqueWeaponCount; i++)
                {
                    GameObject g = Instantiate(uiDisplay.slotTemplate, uiDisplay.transform);
                    g.name = uiDisplay.slotTemplate.name;
                    uiDisplay.weaponSlots.Add(g.transform);
                }
            }
        }
    }

    WeaponData[] FindAllWeaponDataAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:WeaponData");
        List<WeaponData> weaponDataList = new List<WeaponData>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WeaponData weaponData = AssetDatabase.LoadAssetAtPath<WeaponData>(path);
            if (weaponData != null)
            {
                weaponDataList.Add(weaponData);
            }
        }

        // For demonstration purposes, we'll just log the names of the weapons.
        return weaponDataList.ToArray();
    }
}
