using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Gameplay,
        Paused,
        InMap,
        GameOver,
        LevelUp
    }

    public GameState currentState;
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
    int stackedLevelUps = 0;

    [Header("Results Screen Displays")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurvivedDisplay;
    public TMP_Text enemiesKilledDisplay;
    public TMP_Text totalDamageDisplay;
    public TMP_Text totalDPSDisplay;
    public TMP_Text totalGoldDisplay;
    public TMP_Text goldMultiplierDisplay;

    [Header("Stopwatch")]
    public float timeLimit;
    public float stopwatchTime;
    public TMP_Text stopwatchDisplay;

    [Header("DifficultyManager")]
    public DifficultyManager difficultyManager;
    public DifficultyManager.DifficultyStats difficultyIncrement;
    public float difficultyIncrementTime = 600f;

    public GameObject playerObject;
    private PlayerStats playerStats;

    public bool isGameOver { get { return currentState == GameState.GameOver; } }
    public bool choosingUpgrade { get { return currentState == GameState.LevelUp; } }

    [Header("KillsUI")]
    public int killCount = 0;
    public float totalDamageDone = 0;
    public float dps;

    [Header("Gold")]
    public int totalGoldCollected = 0;
    public float goldMultiplier = 1.0f;

    [Header("Score")]
    public TMP_Text scoreText;
    private float score;

    [SerializeField] private float killWeight = 1.0f;
    [SerializeField] private float damageWeight = 0.1f;
    [SerializeField] private float dpsWeight = 0.5f;
    [SerializeField] private float goldWeight = 0.05f;

    [Header("Leaderboard")]
    public LeaderboardFinder leaderboardManager; // Add reference to LeaderboardManager

    public float GetElapsedTime() { return stopwatchTime; }

    void Awake()
    {
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
        StartCoroutine(UpdateKillCountTextCoroutine());
        StartCoroutine(UpdateDamageTextCoroutine());
        StartCoroutine(UpdateDPSTextCoroutine());

        playerStats = playerObject.GetComponent<PlayerStats>();
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
            case GameState.Paused:
                CheckForPauseAndResume();
                UpdateStopwatch();
                UpdateGoldMultiplier();
                break;
            case GameState.GameOver:
                CheckForGameOverInput();
                break;
            case GameState.LevelUp:
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }

        CalculateScore();
        UpdateScoreUI();
    }

    private void UpdateGoldMultiplier()
    {
        if (playerStats != null)
        {
            goldMultiplier = playerStats.ActualStats.luck;
        }
    }

    private void CalculateScore()
    {
        score = (killCount * killWeight) +
                (totalDamageDone * damageWeight) +
                (dps * dpsWeight) +
                (totalGoldCollected * goldWeight);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("F2");
        }
    }

    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if (textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(textObj, duration);

        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnownPosition = target.position;
        while (t < duration)
        {
            if (!rect) break;

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            if (target) lastKnownPosition = target.position;

            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnownPosition + new Vector3(0, yOffset));

            yield return w;
            t += Time.deltaTime;
        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!instance.damageTextCanvas) return;

        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(
            text, target, duration, speed
        ));
    }

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
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            MapScreen.SetActive(false);
        }
    }

    public void MapScreenPauce()
    {
        if (currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            MapScreen.SetActive(true);
        }
    }

    public void InvintoryScreenPauce()
    {
        if (currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            MapScreen.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            MapScreen.SetActive(false);
        }
    }

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
    }

    void CheckForGameOverInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.GameOver)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Base");
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
        totalGoldDisplay.text = $"{(totalGoldCollected):F0}";
        goldMultiplierDisplay.text = $"{goldMultiplier}x";

        ChangeState(GameState.GameOver);
        Time.timeScale = 0f;
        DisplayResults();

        if (leaderboardManager != null)
        {
            leaderboardManager.UploadScore((int)score); // Upload the score when game is over
        }
        SteamLeaderboardManager.UpdateScore((int)score);
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(PlayerStats chosenCharacter)
    {
        chosenCharacterImage.sprite = chosenCharacter.Icon;
        chosenCharacterName.text = (" Starting Weapon:" + chosenCharacter.StartingWeapon.name);
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
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);

        if (levelUpScreen.activeSelf) stackedLevelUps++;
        else
        {
            levelUpScreen.SetActive(true);
            Time.timeScale = 0f;
            playerObject.SendMessage("RemoveAndApplyUpgrades");
        }
    }

    public void EndLevelUp()
    {
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);

        if (stackedLevelUps > 0)
        {
            stackedLevelUps--;
            StartLevelUp();
        }
    }

    public void IncrementKillCount()
    {
        killCount++;
    }

    public int GetKillCount()
    {
        return killCount;
    }

    public void IncrementTotalDamageDone(float dmg)
    {
        totalDamageDone += dmg;
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
            totalDPSDisplay.text = "" + dps.ToString("F2");
        }
        else
        {
            Debug.LogWarning("Stopwatch time is not greater than 0.");
        }
    }

    IEnumerator UpdateDamageTextCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (totalDamageDone > 0)
            {
                totalDamageDisplay.text = "" + GetTotalDamageDoneFormatted().ToString();
            }
        }
    }

    IEnumerator UpdateDPSTextCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            UpdateDPS();
        }
    }

    IEnumerator UpdateKillCountTextCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (killCount > 0)
            {
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

    public void IncrementGold(int amount)
    {
        int adjustedAmount = Mathf.RoundToInt(amount * goldMultiplier);
        totalGoldCollected += adjustedAmount;

        if (PlayerGold.instance != null)
        {
            PlayerGold.instance.AddGold(adjustedAmount);
        }
    }
}
