using UnityEngine;
using TMPro;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole instance;

    [SerializeField] TextMeshProUGUI consoleText;
    private bool isInitialized = false;

    void Awake()
    {
        if (DebugConsole.instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DebugConsole.instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the console persistent across scenes
            Application.logMessageReceived += HandleLog;
        }
    }

    void OnDestroy()
    {
        if (DebugConsole.instance == this)
        {
            Application.logMessageReceived -= HandleLog;
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string message = $"{type}: {logString}";
        if (type == LogType.Exception || type == LogType.Error)
        {
            message += $"\n{stackTrace}";
        }
        //DebugConsole.instance.ClearLog();
        Log(message);
    }
    public void ClearLog()
    {
        consoleText.text = string.Empty;
        isInitialized = false;
    }

    private void Log(string logString)
    {
        if (!isInitialized)
        {
            consoleText.text = string.Empty;
            isInitialized = true;
        }

        consoleText.text = logString + "\n" + consoleText.text;
    }
}
