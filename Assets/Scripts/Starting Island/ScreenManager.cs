using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject weaponsScreen;
    public GameObject runesScreen;
    public GameObject settingsScreen;

    public GameObject currentScreen;

    void Start()
    {
        // Set all screens to inactive when the script first loads
        if (weaponsScreen != null) weaponsScreen.SetActive(false);
        if (runesScreen != null) runesScreen.SetActive(false);
        if (settingsScreen != null) settingsScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }
    }

    public void OpenScreen(GameObject screen)
    {
        if (currentScreen != null)
        {
            currentScreen.SetActive(false);
        }

        screen.SetActive(true);
        currentScreen = screen;
    }

    public void CloseCurrentScreen()
    {
        if (currentScreen != null)
        {
            currentScreen.SetActive(false);
            currentScreen = null;
        }
    }

    private void HandleEscapeKey()
    {
        if (currentScreen != null)
        {
            CloseCurrentScreen();
        }
        else
        {
            OpenScreen(settingsScreen);
        }
    }

    public void OpenWeaponsScreen()
    {
        OpenScreen(weaponsScreen);
    }

    public void OpenRunesScreen()
    {
        OpenScreen(runesScreen);
    }

    public void OpenSettingsScreen()
    {
        OpenScreen(settingsScreen);
    }
}
