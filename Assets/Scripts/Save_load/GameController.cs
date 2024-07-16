using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public RuneInventory runeInventory;
    private SaveLoadManager saveLoadManager;

    private void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();

        // Load rune bags at the start of any scene
        if (saveLoadManager != null)
        {
            saveLoadManager.LoadRuneBags(runeInventory);
        }
        else
        {
            Debug.LogWarning("SaveLoadManager not found. Cannot load rune bags.");
        }

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnApplicationQuit()
    {
        // Save rune bags when the application quits
        if (saveLoadManager != null && runeInventory != null)
        {
            saveLoadManager.SaveRuneBags(runeInventory.runeBag, runeInventory.equippedRuneBag);
        }
    }

    private void OnSceneUnloaded(Scene current)
    {
        // Save rune bags when the scene is unloaded
        if (saveLoadManager != null && runeInventory != null)
        {
            saveLoadManager.SaveRuneBags(runeInventory.runeBag, runeInventory.equippedRuneBag);
        }
    }
}
