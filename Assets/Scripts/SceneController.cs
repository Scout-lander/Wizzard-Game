using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private SaveLoadManager saveLoadManager;
    private RuneInventory runeInventory;

    private void Awake()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        runeInventory = FindObjectOfType<RuneInventory>();
    }

    public void SceneChange(string name)
    {
        if (saveLoadManager != null && runeInventory != null)
        {
            saveLoadManager.SaveRuneBags(runeInventory.runeBag, runeInventory.equippedRuneBag);
        }
        else
        {
            Debug.LogWarning("SaveLoadManager or RuneInventoryNew not found. Cannot save rune bags.");
        }

        SceneManager.LoadScene(name);
        Time.timeScale = 1;
    }
}
