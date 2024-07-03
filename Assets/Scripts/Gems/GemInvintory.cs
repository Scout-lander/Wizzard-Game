using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemInventory : MonoBehaviour
{
    public GemBag gemBag;  // This is your ScriptableObject that holds the gems.
    public TextMeshProUGUI notificationText;  // Reference to the TMP text component

    public void AddGem(GemData gem)
    {
        if (gemBag.AddGem(gem))
        {
            Debug.Log("Gem added to inventory: " + gem.gemName);
        }
        else
        {
            //Debug.Log("Failed to add gem to inventory: Inventory might be full or gem already exists.");
            ShowNotification("Your Bag is full. " + gem.gemName + " destroyed");
            Destroy(gem);  // Assuming gem is a GameObject. Adjust if it's not.
        }
    }

    private void ShowNotification(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(3f);  // Show the message for 2 seconds
        notificationText.gameObject.SetActive(false);
    }
}
