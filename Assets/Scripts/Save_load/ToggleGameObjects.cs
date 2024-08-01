using UnityEngine;
using UnityEngine.UI;

public class ToggleGameObjects : MonoBehaviour
{
    public Toggle toggle;  // Reference to the UI Toggle
    public GameObject[] objectsToHide;  // Reference to the GameObjects to hide
    public GameObject[] objectsToShow;  // Reference to the GameObjects to show
    private SaveLoadManager saveLoadManager;
    public bool isToggleOn;  // Public boolean to reflect the toggle state

    void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        if (saveLoadManager == null)
        {
            Debug.LogError("SaveLoadManager not found in the scene.");
            return;
        }

        // Load the saved toggle state and update the toggle and game objects
        bool savedToggleState = saveLoadManager.LoadToggleState();
        SetToggleState(savedToggleState);

        // Add a listener to call UpdateGameObjects whenever the toggle's state changes
        toggle.onValueChanged.AddListener(delegate {
            SetToggleState(toggle.isOn);
            saveLoadManager.SaveToggleState(toggle.isOn);
        });
    }

    public void SetToggleState(bool isOn)
    {
        toggle.isOn = isOn;
        UpdateGameObjects(isOn);
        isToggleOn = isOn;  // Update the boolean variable
    }

    void UpdateGameObjects(bool isOn)
    {
        if (objectsToHide != null)
        {
            foreach (GameObject obj in objectsToHide)
            {
                obj.SetActive(!isOn);
            }
        }

        if (objectsToShow != null)
        {
            foreach (GameObject obj in objectsToShow)
            {
                obj.SetActive(isOn);
            }
        }
    }
}
