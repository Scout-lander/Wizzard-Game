using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Define the different states of the game
    public enum GameState
    {
        Gameplay,
        Paused,
        InMap,
        Inventory,
        GameOver,
        LevelUp
    }

    // Store the current state of the game
    public GameState currentState;

    // Store the previous state of the game before it was paused
    public GameState previousState;

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;
    public GameObject MapScreen;
    public GameObject InvintoryScreen;
    int stackedLevelUps = 0; // If we try to StartLevelUp() multiple times.

    [Header("Results Screen Displays")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurvivedDisplay;
    public TMP_Text enemiesKilledDisplay;
    public TMP_Text totalDamageDisplay;
    public TMP_Text totalDPSDisplay;
    //public List<Image> chosenWeaponsUI = new List<Image>(6);
    //public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    [Header("Stopwatch")]
    public float timeLimit; // The time limit in seconds
    public float stopwatchTime; // The current time elapsed since the stopwatch started
    public TMP_Text stopwatchDisplay;
    
    [Header("DifficultyManager")]
    public DifficultyManager difficultyManager;
    public DifficultyManager.DifficultyStats difficultyIncrement;
    public float difficultyIncrementTime = 600f;

    // Reference to the player's game object
    public GameObject playerObject;

    // Getters for parity with older scripts.
    public bool isGameOver { get { return currentState == GameState.Paused; } }
    public bool choosingUpgrade { get { return currentState == GameState.LevelUp; } }

    [Header("KillsUI")]
    public int killCount = 0;
    public float totalDamageDone = 0;
    public float dps;

    public float GetElapsedTime() { return stopwatchTime; }

    void Awake()
    {
        //Warning check to see if there is another singleton of this kind already in the game
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }

        DisableScreens();
        StartCoroutine(IncreaseDifficultyOverTime());
        StartCoroutine(UpdateKillCountTextCoroutine()); // Start the coroutine to update kill count text
        StartCoroutine(UpdateDamageTextCoroutine()); // Start the coroutine to update kill count text
        StartCoroutine(UpdateDPSTextCoroutine()); // Start coroutine to update DPS text
    }

    void Update()
    {
        // Define the behavior for each state
        switch (currentState)
        {
            case GameState.Gameplay:
            case GameState.Paused:
                // Code for the gameplay state
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;
            case GameState.GameOver:
            case GameState.LevelUp:
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        // Start generating the floating text.
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        // Makes sure this is destroyed after the duration finishes.
        Destroy(textObj, duration);

        // Parent the generated text object to the canvas.
        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        // Pan the text upwards and fade it away over time.
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnownPosition = target.position;
        while (t < duration)
        {
            // If the RectTransform is missing for whatever reason, end this loop.
            if (!rect) break;

            // Fade the text to the right alpha value.
            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            // Update the enemy's position if it is still around.
            if (target) lastKnownPosition = target.position;

            // Pan the text upwards.
            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnownPosition + new Vector3(0, yOffset));

            // Wait for a frame and update the time.
            yield return w;
            t += Time.deltaTime;
        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        // If the canvas is not set, end the function so we don't
        // generate any floating text.
        if (!instance.damageTextCanvas) return;

        // Find a relevant camera that we can use to convert the world
        // position to a screen position.
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(
            text, target, duration, speed
        ));
    }

    // Define the method to change the state of the game
    public void ChangeState(GameState newState)
    {
        previousState = currentState;
        currentState = newState;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Stop the game
            pauseScreen.SetActive(true); // Enable the pause screen
            MapScreen.SetActive(false); // Sets Map to not be open when first opening the pause screen
            InvintoryScreen.SetActive(false);
        }
    }
    public void MapScreenPauce()
    {
        if (currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Stop the game
            pauseScreen.SetActive(true); // Enable the pause screen
            MapScreen.SetActive(true); // Sets Map to not be open when first opening the pause screen
            InvintoryScreen.SetActive(false);
        }
    }

    public void InvintoryScreenPauce()
    {
        if (currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Stop the game
            pauseScreen.SetActive(true); // Enable the pause screen
            MapScreen.SetActive(false); // Sets Map to not be open when first opening the pause screen
            InvintoryScreen.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // Resume the game
            pauseScreen.SetActive(false); //Disable the pause screen
            MapScreen.SetActive(false);
            InvintoryScreen.SetActive(false);
        }
    }

    // Define the method to check for pause and resume input
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                MapScreenPauce();
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                InvintoryScreenPauce();
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
        MapScreen.SetActive(false);
    }

    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text;

        // Set the Game Over variables here.
        ChangeState(GameState.GameOver);
        Time.timeScale = 0f; //Stop the game entirely
        DisplayResults();
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(PlayerStats chosenCharacter)
    {
        chosenCharacterImage.sprite = chosenCharacter.Icon;
        chosenCharacterName.text = (" Starting Weapon:" + chosenCharacter.Name);
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopwatchDisplay();

        if (stopwatchTime >= timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }

    void UpdateStopwatchDisplay()
    {
        // Calculate the number of minutes and seconds that have elapsed
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        // Update the stopwatch text to display the elapsed time
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);

        // If the level up screen is already active, record it.
        if(levelUpScreen.activeSelf) stackedLevelUps++;
        else
        {
            levelUpScreen.SetActive(true);
            Time.timeScale = 0f; //Pause the game for now
            playerObject.SendMessage("RemoveAndApplyUpgrades");
        }
    }

    public void EndLevelUp()
    {
        Time.timeScale = 1f;    //Resume the game
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
        
        if(stackedLevelUps > 0)
        {
            stackedLevelUps--;
            StartLevelUp();
        }
    }

    public void IncrementKillCount()
    {
        killCount++;
        //QuestManager.instance.UpdateQuests(QuestType.Kills, killCount);
    }

    // Method to get the current kill count
    public int GetKillCount()
    {
        return killCount;
    }
    
    public void IncrementTotalDamageDone(float dmg)
    {
        totalDamageDone += dmg;
        //QuestManager.instance.UpdateQuests(QuestType.DamageDone, (int)totalDamageDone);
    }

    public string GetTotalDamageDoneFormatted()
    {
        if (totalDamageDone >= 1000)
        {
            float damageInK = totalDamageDone / 1000f;
            return damageInK.ToString("0.00") + "k";
        }
        else
        {
            return totalDamageDone.ToString("0");
        }
    }

    public void UpdateDPS()
    {
        if (stopwatchTime > 0)
        {
            dps = totalDamageDone / stopwatchTime;
            totalDPSDisplay.text = "" + dps.ToString("F2"); // Display DPS with 2 decimal places
            //QuestManager.instance.UpdateQuests(QuestType.DPS, (int)dps);
        }
        else
        {
            Debug.LogWarning("Stopwatch time is not greater than 0.");
        }
    }

    // Method to update the kill count text
    IEnumerator UpdateDamageTextCoroutine()
    {
        while (true) // Infinite loop to update the text every second
        {
            yield return new WaitForSeconds(1f); // Wait for one second

            if (totalDamageDone > 0) // Check if Damage is greater then 0
            {
                // Update the kill count text with the current kill count
                totalDamageDisplay.text = "" + GetTotalDamageDoneFormatted().ToString();
            }
        }
    }

    IEnumerator UpdateDPSTextCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Wait for one second

            UpdateDPS(); // Update DPS text
        }
    }

    IEnumerator UpdateKillCountTextCoroutine()
    {
        while (true) // Infinite loop to update the text every second
        {
            yield return new WaitForSeconds(1f); // Wait for one second

            if (killCount > 0) // Check if Kill Count is Greater then 0
            {
                // Update the kill count text with the current kill count
                enemiesKilledDisplay.text = "" + GetKillCount().ToString();
            }
        }
    }

    private IEnumerator IncreaseDifficultyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncrementTime);
            difficultyManager.IncreaseDifficulty(difficultyIncrement);
        }
    }
}
