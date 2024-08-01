using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public Toggle fullscreenTog, vSyncTog;
    public Button quitButton;
    public Button titleScreenButton; // New button for title screen
    public Button megapackButton; // New button for Megapack screen

    void Start()
    {
        // Initialize UI elements with current settings
        fullscreenTog.isOn = Screen.fullScreen;
        vSyncTog.isOn = QualitySettings.vSyncCount > 0;

        // Add listener to quit button
        quitButton.onClick.AddListener(QuitGame);

        // Add listener to title screen button
        titleScreenButton.onClick.AddListener(GoToTitleScreen);

        // Add listener to Megapack screen button
        megapackButton.onClick.AddListener(GoToMegapackScreen);
    }

    public void ApplyGraphics()
    {
        // Apply FullScreen
        Screen.fullScreen = fullscreenTog.isOn;

        // Apply VSync
        QualitySettings.vSyncCount = vSyncTog.isOn ? 1 : 0;

        // Apply Resolution to 1080p
        Screen.SetResolution(1920, 1080, fullscreenTog.isOn);
    }

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();

        // For editor mode
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void GoToTitleScreen()
    {
        // Change to the title screen scene
        SceneManager.LoadScene("TitleScreen"); // Ensure "TitleScreen" is the correct name of your title screen scene
    }

    public void GoToMegapackScreen()
    {
        // Change to the Megapack scene
        SceneManager.LoadScene("Megapack"); // Ensure "Megapack" is the correct name of your Megapack scene
    }
}
