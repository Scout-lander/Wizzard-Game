using UnityEngine;
using TMPro;

public class GoldDisplay : MonoBehaviour
{
    public TMP_Text goldText; // Reference to the TMP text component

    private void OnEnable()
    {
        // Subscribe to the gold update event
        if (PlayerGold.instance != null)
        {
            PlayerGold.instance.onGoldChanged += UpdateGoldText;
            UpdateGoldText(PlayerGold.instance.totalGold); // Initialize the gold text
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the gold update event
        if (PlayerGold.instance != null)
        {
            PlayerGold.instance.onGoldChanged -= UpdateGoldText;
        }
    }

    // Method to update the TMP text component with the current gold amount
    private void UpdateGoldText(int totalGold)
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {totalGold}";
        }
        else
        {
            Debug.LogWarning("Gold text component is not assigned.");
        }
    }
}
