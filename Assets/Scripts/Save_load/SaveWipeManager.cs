using UnityEngine;
using UnityEngine.UI;

public class SaveWipeManager : MonoBehaviour
{
    private const string WipeFlagKey = "NeedWipe"; // Key for the PlayerPrefs flag
    public bool shouldWipeOnFirstRun = false; // Public flag to set before building the game

    public GameObject confirmationWindow; // Reference to the confirmation window UI
    public Button wipeButton; // Reference to the wipe button
    public Button confirmButton; // Reference to the confirm button
    public Button cancelButton; // Reference to the cancel button

    void Awake()
    {
        // Check if the wipe flag is set and perform the wipe if necessary
        if (shouldWipeOnFirstRun && PlayerPrefs.GetInt(WipeFlagKey, 0) == 0)
        {
            PerformWipe();
        }

        // Initialize the button listeners
        if (wipeButton != null)
        {
            wipeButton.onClick.AddListener(ShowConfirmationWindow);
        }
        
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(PerformWipe);
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(HideConfirmationWindow);
        }

        // Ensure the confirmation window is initially hidden
        if (confirmationWindow != null)
        {
            confirmationWindow.SetActive(false);
        }
    }

    // Method to perform the wipe
    private void PerformWipe()
    {
        Debug.Log("Wipe flag is set. Performing wipe...");

        // Perform the wipe operation here
        WipeRuneData();

        // Set the PlayerPrefs flag to indicate that the wipe has been performed
        PlayerPrefs.SetInt(WipeFlagKey, 1);
        PlayerPrefs.Save();

        // Hide the confirmation window after performing the wipe
        HideConfirmationWindow();
    }

    // Method to wipe rune data
    private void WipeRuneData()
    {
        // Assuming you have a SaveLoadManager with a method to delete the rune save files
        SaveLoadManager saveLoadManager = FindObjectOfType<SaveLoadManager>();
        if (saveLoadManager != null)
        {
            saveLoadManager.DeleteRuneSaveFiles();
            Debug.Log("Player's rune data has been wiped.");
        }
        else
        {
            Debug.LogError("SaveLoadManager not found. Cannot wipe rune data.");
        }
    }

    // Method to show the confirmation window
    private void ShowConfirmationWindow()
    {
        if (confirmationWindow != null)
        {
            confirmationWindow.SetActive(true);
        }
    }

    // Method to hide the confirmation window
    private void HideConfirmationWindow()
    {
        if (confirmationWindow != null)
        {
            confirmationWindow.SetActive(false);
        }
    }
}
