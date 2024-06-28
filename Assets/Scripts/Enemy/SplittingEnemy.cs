using System.Collections;
using UnityEngine;

public class SplittingEnemy : MonoBehaviour
{
    public GameObject splittingEffect;
    public GameObject enemySplitPrefab;

    // Stats class to allow managing all stats as a single object.
    [System.Serializable]
    public class Stats
    {
        public float spawnOffsetDistance = 0.8f;
        public int numberOfSplits = 2; // Default number of splits
        public float splitSpawnDelay = 0.5f;

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.numberOfSplits += s2.numberOfSplits;
            s1.splitSpawnDelay += s2.splitSpawnDelay;
            s1.spawnOffsetDistance += s2.spawnOffsetDistance;
            return s1;
        }
    }


    public Stats baseStats;
    [SerializeField] Stats actualStats;

    public Stats ActualStats
    {
        get { return actualStats; }
    }

    private void Start()
    {
        actualStats = baseStats;

        // Adjust enemy parameters based on difficulty level
        int difficultyLevel = DifficultyManager.Instance.CurrentDifficultyLevel;
        AdjustStatsBasedOnDifficulty(difficultyLevel);
    }

    private IEnumerator SpawnEnemiesWithDelay(Vector3 currentPosition)
    {
        yield return new WaitForSeconds(actualStats.splitSpawnDelay);
           
        for (int i = 0; i < actualStats.numberOfSplits; i++)
        {
            Vector3 offset = Random.insideUnitSphere.normalized * actualStats.spawnOffsetDistance;
            Vector3 spawnPosition = currentPosition + offset;
            Instantiate(enemySplitPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void OnKill()
    {
       if (splittingEffect != null)
        {
            Instantiate(splittingEffect, transform.position, Quaternion.identity);
        }
        StartCoroutine(SpawnEnemiesWithDelay(transform.position));
    }

    private void AdjustStatsBasedOnDifficulty(int difficultyLevel)
    {
        // Example of how you might adjust stats based on difficulty level
        actualStats.numberOfSplits += difficultyLevel; // Increase splits with difficulty
        actualStats.splitSpawnDelay = Mathf.Max(0.1f, actualStats.splitSpawnDelay - 0.1f * difficultyLevel); // Reduce delay with difficulty
    }
    
    public void ApplyDifficultyScaling(DifficultyManager.DifficultyStats difficultyStats)
    {
        actualStats += difficultyStats.splittingEnemyStats;
    }
}
