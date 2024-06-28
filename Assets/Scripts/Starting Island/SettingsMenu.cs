using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Toggle fullscreenTog, vSyncTog;
    public Button quitButton;

    void Start()
    {
        // Initialize UI elements with current settings
        fullscreenTog.isOn = Screen.fullScreen;
        vSyncTog.isOn = QualitySettings.vSyncCount > 0;

        // Add listener to quit button
        quitButton.onClick.AddListener(QuitGame);
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
}
