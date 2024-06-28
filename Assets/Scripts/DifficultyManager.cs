using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [System.Serializable]
    public class DifficultyStats
    {
        public SummoningEnemy.Stats summoningEnemyStats;
        public BombEnemy.Stats bombEnemyStats;
        public SplittingEnemy.Stats splittingEnemyStats;
        public DashingEnemy.Stats dashingEnemyStats;
        public ShootingEnemy.Stats shootingEnemyStats;
        public EnemyStats.Stats enemyStats; // Add reference to EnemyStats stats

        [Header("Boss")]
        public CrimsonCrusader.Stats crimsonCrusaderStats;

        public static DifficultyStats operator +(DifficultyStats ds1, DifficultyStats ds2)
        {
            ds1.summoningEnemyStats += ds2.summoningEnemyStats;
            ds1.bombEnemyStats += ds2.bombEnemyStats;
            ds1.splittingEnemyStats += ds2.splittingEnemyStats;
            ds1.dashingEnemyStats += ds2.dashingEnemyStats;
            ds1.shootingEnemyStats += ds2.shootingEnemyStats;
            ds1.enemyStats += ds2.enemyStats; // Add for EnemyStats stats
            ds1.crimsonCrusaderStats += ds2.crimsonCrusaderStats;
            return ds1;
        }
    }

    public DifficultyStats baseDifficultyStats;
    private DifficultyStats currentDifficultyStats;

    public int CurrentDifficultyLevel { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentDifficultyStats = baseDifficultyStats;
    }

    public void IncreaseDifficulty(DifficultyStats difficultyIncrement)
    {
        currentDifficultyStats += difficultyIncrement;
        CurrentDifficultyLevel++;

        // Apply new difficulty settings to all enemies
        SummoningEnemy[] summoningEnemies = FindObjectsOfType<SummoningEnemy>();
        foreach (SummoningEnemy summoningEnemy in summoningEnemies)
        {
            summoningEnemy.ApplyDifficultyScaling(currentDifficultyStats);
        }

        BombEnemy[] bombEnemies = FindObjectsOfType<BombEnemy>();
        foreach (BombEnemy bombEnemy in bombEnemies)
        {
            bombEnemy.ApplyDifficultyScaling(currentDifficultyStats);
        }

        SplittingEnemy[] splittingEnemies = FindObjectsOfType<SplittingEnemy>();
        foreach (SplittingEnemy splittingEnemy in splittingEnemies)
        {
            splittingEnemy.ApplyDifficultyScaling(currentDifficultyStats);
        }

        DashingEnemy[] dashingEnemies = FindObjectsOfType<DashingEnemy>();
        foreach (DashingEnemy dashingEnemy in dashingEnemies)
        {
            dashingEnemy.ApplyDifficultyScaling(currentDifficultyStats);
        }

        ShootingEnemy[] shootingEnemies = FindObjectsOfType<ShootingEnemy>();
        foreach (ShootingEnemy shootingEnemy in shootingEnemies)
        {
            shootingEnemy.ApplyDifficultyScaling(currentDifficultyStats);
        }

        EnemyStats[] enemyStats = FindObjectsOfType<EnemyStats>(); // Find all enemy stats
        foreach (EnemyStats enemyStat in enemyStats)
        {
            enemyStat.ModifyActualStats(difficultyIncrement.enemyStats); // Modify enemy stats
        }

        CrimsonCrusader[] crimsonCrusaders = FindObjectsOfType<CrimsonCrusader>();
        foreach (CrimsonCrusader crimsonCrusader in crimsonCrusaders)
        {
            crimsonCrusader.ApplyDifficultyScaling(currentDifficultyStats);
        }
    }
    // Method to get the current difficulty stats
    public DifficultyStats GetCurrentDifficultyStats()
    {
        return currentDifficultyStats;
    }
}
