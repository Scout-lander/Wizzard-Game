using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GemInventory : MonoBehaviour
{
    public GemBag gemBag;  // This is your ScriptableObject that holds the gems.

    public void AddGem(GemData gem)
    {
        if (gemBag.AddGem(gem))
        {
            Debug.Log("Gem added to inventory: " + gem.gemName);
        }
        else
        {
            Debug.Log("Failed to add gem to inventory: Inventory might be full or gem already exists.");
        }
    }
}
