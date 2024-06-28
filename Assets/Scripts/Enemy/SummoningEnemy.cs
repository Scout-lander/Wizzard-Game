using System.Collections;
using UnityEngine;

public class SummoningEnemy : MonoBehaviour
{
    public Color flashColor = new Color(0, 1, 0, 1);
    public ParticleSystem summoningEffect;
    public GameObject[] enemyPrefabsToSummon;

    // Stats class to manage all stats as a single object.
    [System.Serializable]
    public class Stats
    {
        public float summoningDuration = 3f;
        public float minSummonAmount = 1f;
        public float maxSummonAmount = 4f;
        public float summoningDistance = 5f;
        public float summonCooldown = 10f;

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.summoningDuration += s2.summoningDuration;
            s1.minSummonAmount += s2.minSummonAmount;
            s1.maxSummonAmount += s2.maxSummonAmount;
            s1.summoningDistance += s2.summoningDistance;
            s1.summonCooldown += s2.summonCooldown;
            return s1;
        }
    }

    public Stats baseStats;
    [SerializeField] private Stats actualStats;

    public Stats ActualStats
    {
        get { return actualStats; }
    }

    private bool isSummoning = false;
    private bool isOnCooldown = false;
    private Transform player;
    private Animator animator;
    private SpriteRenderer sr;
    private EnemyMovement enemyMovement; // Reference to the EnemyMovement component

    private int difficultyLevel;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovement>(); // Get the EnemyMovement component
        actualStats = baseStats;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SummonCoroutine());

        AdjustDifficultyLevel(DifficultyManager.Instance.CurrentDifficultyLevel);
    }

    private IEnumerator SummonCoroutine()
    {
        while (true)
        {
            yield return null;

            if (player != null && !isOnCooldown && !isSummoning && Vector2.Distance(transform.position, player.position) <= actualStats.summoningDistance)
            {
                isSummoning = true;
                isOnCooldown = true;
                animator.SetBool("Summoning", true);

                if (summoningEffect != null)
                {
                    Instantiate(summoningEffect, transform.position, Quaternion.identity);
                }

                PauseMovement(true);

                // Flash color while summoning
                StartCoroutine(FlashColor());

                yield return new WaitForSeconds(actualStats.summoningDuration);

                SpawnEnemies();

                animator.SetBool("Summoning", false);
                PauseMovement(false);
                isSummoning = false;

                yield return new WaitForSeconds(actualStats.summonCooldown);
                isOnCooldown = false;
            }
        }
    }

    private IEnumerator FlashColor()
    {
        float elapsedTime = 0f;

        while (elapsedTime < actualStats.summoningDuration)
        {
            sr.color = (elapsedTime % 0.5f < 0.25f) ? flashColor : Color.white;
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        sr.color = Color.white;
    }

    private void PauseMovement(bool pause)
    {
        if (enemyMovement != null)
        {
            enemyMovement.enabled = !pause; // Disable or enable the EnemyMovement component
        }
    }

    private void SpawnEnemies()
    {
        if (enemyPrefabsToSummon == null || enemyPrefabsToSummon.Length == 0)
        {
            Debug.LogError("No enemy prefabs to summon!");
            return;
        }

        int summonAmount = Random.Range((int)actualStats.minSummonAmount, (int)actualStats.maxSummonAmount + 1);

        for (int i = 0; i < summonAmount; i++)
        {
            GameObject enemyPrefabToSummon = enemyPrefabsToSummon[Random.Range(0, enemyPrefabsToSummon.Length)];
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 2f;
            Instantiate(enemyPrefabToSummon, spawnPosition, Quaternion.identity);
        }
    }

    public void ApplyDifficultyScaling(DifficultyManager.DifficultyStats difficultyStats)
    {
        actualStats += difficultyStats.summoningEnemyStats;
    }

    public void AdjustDifficultyLevel(int difficultyLevel)
    {
        this.difficultyLevel = difficultyLevel;

        // You can customize these values based on your difficulty scaling strategy
        actualStats.summoningDuration = baseStats.summoningDuration * (1 - difficultyLevel * 0.05f);
        actualStats.minSummonAmount = baseStats.minSummonAmount + difficultyLevel * 0.5f;
        actualStats.maxSummonAmount = baseStats.maxSummonAmount + difficultyLevel * 0.5f;
        actualStats.summoningDistance = baseStats.summoningDistance * (1 + difficultyLevel * 0.1f);
        actualStats.summonCooldown = baseStats.summonCooldown * (1 - difficultyLevel * 0.05f);
    }
}
