using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrimsonCrusader : MonoBehaviour
{
    [Header("Meteor")]
    public GameObject meteorPrefab;

    [Header("Summon")]
    public GameObject minionPrefab;
    public Transform[] minionSpawnPoints;
    public ParticleSystem minionSpawnParticles;

    [Header("Teleport")]
    public ParticleSystem teleportParticlesPrefab;

    // Cooldown timers
    private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>();

    private Animator bossAnimator;
    private EnemyStats enemyStats;
    private Transform player;
    private bool isWhirlwindActive = false;
    private bool hasUsedWhirlwind = false;

    [System.Serializable]
    public class Stats
    {
        [Header("Meteor")]
        public float meteorShowerCooldown = 20f;
        public int minMeteorCount = 3;
        public int maxMeteorCount = 6;

        [Header("Summon")]
        public float summonMinionsCooldown = 15f;
        public int minMinionsToSpawn = 2;
        public int maxMinionsToSpawn = 5;

        [Header("Teleport")]
        public float teleportCooldown = 10f;

        [Header("Whirlwind")]
        public float whirlwindRadius = 3f;
        public float whirlwindDamage = 10f;
        public float whirlwindDuration = 5f;
        public float whirlwindCooldown = 30f;
        public float whirlwindMoveSpeedMultiplier = 0.5f;

        public static Stats operator +(Stats s1, Stats s2)
        {
            return new Stats
            {
                meteorShowerCooldown = s1.meteorShowerCooldown + s2.meteorShowerCooldown,
                minMeteorCount = s1.minMeteorCount + s2.minMeteorCount,
                maxMeteorCount = s1.maxMeteorCount + s2.maxMeteorCount,
                summonMinionsCooldown = s1.summonMinionsCooldown + s2.summonMinionsCooldown,
                minMinionsToSpawn = s1.minMinionsToSpawn + s2.minMinionsToSpawn,
                maxMinionsToSpawn = s1.maxMinionsToSpawn + s2.maxMinionsToSpawn,
                teleportCooldown = s1.teleportCooldown + s2.teleportCooldown,
                whirlwindRadius = s1.whirlwindRadius + s2.whirlwindRadius,
                whirlwindDamage = s1.whirlwindDamage + s2.whirlwindDamage,
                whirlwindDuration = s1.whirlwindDuration + s2.whirlwindDuration,
                whirlwindCooldown = s1.whirlwindCooldown + s2.whirlwindCooldown,
                whirlwindMoveSpeedMultiplier = s1.whirlwindMoveSpeedMultiplier + s2.whirlwindMoveSpeedMultiplier
            };
        }
    }

    public Stats baseStats;
    [SerializeField] Stats actualStats;

    public Stats ActualStats => actualStats;

    private void Start()
    {
        bossAnimator = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        actualStats = baseStats;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize cooldown timers
        cooldownTimers["Teleport"] = 0f;
        cooldownTimers["MeteorShower"] = 0f;
        cooldownTimers["SummonMinions"] = 0f;
        cooldownTimers["Whirlwind"] = 0f;
    }

    private void Update()
    {
        UpdateCooldownTimers();
        ExecuteBossAI();
    }

    private void UpdateCooldownTimers()
    {
        var keys = new List<string>(cooldownTimers.Keys);
        foreach (var key in keys)
        {
            cooldownTimers[key] -= Time.deltaTime;
        }
    }

    private void ExecuteBossAI()
    {
        if (hasUsedWhirlwind && CanUseAbility("Teleport"))
        {
            Teleport();
            ResetCooldown("Teleport", actualStats.teleportCooldown);
        }

        if (CanUseAbility("MeteorShower") && IsPlayerFar())
        {
            MeteorShower();
            ResetCooldown("MeteorShower", actualStats.meteorShowerCooldown);
        }

        if (CanUseAbility("SummonMinions") && IsPlayerClose())
        {
            SummonMinions();
            ResetCooldown("SummonMinions", actualStats.summonMinionsCooldown);
        }

        if (CanUseAbility("Whirlwind") && IsPlayerClose())
        {
            Whirlwind();
            ResetCooldown("Whirlwind", actualStats.whirlwindCooldown);
        }
    }

    private bool CanUseAbility(string ability) => cooldownTimers[ability] <= 0f;
    private bool IsPlayerClose() => Vector2.Distance(transform.position, player.position) < 5f;
    private bool IsPlayerFar() => Vector2.Distance(transform.position, player.position) > 7f;

    private void ResetCooldown(string ability, float cooldown)
    {
        cooldownTimers[ability] = cooldown;
    }

    public void Teleport()
    {
        if (isWhirlwindActive) return;

        bossAnimator.SetBool("Teleporting", true);
        Vector3 newPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
        Instantiate(teleportParticlesPrefab, transform.position, Quaternion.identity);
        transform.position = newPosition;
        Instantiate(teleportParticlesPrefab, transform.position, Quaternion.identity);
        hasUsedWhirlwind = false;
        StartCoroutine(ResetAnimatorBool("Teleporting"));
    }

    public void MeteorShower()
    {
        bossAnimator.SetBool("CastingMeteor", true);
        StartCoroutine(UseMeteorShower());
        StartCoroutine(ResetAnimatorBool("CastingMeteor"));
    }

    private IEnumerator UseMeteorShower()
    {
        int meteorCount = Random.Range(actualStats.minMeteorCount, actualStats.maxMeteorCount + 1);

        for (int i = 0; i < meteorCount; i++)
        {
            Vector3 meteorPosition = player.position + new Vector3(Random.Range(-5f, 5f), 10f, 0f);
            GameObject meteor = Instantiate(meteorPrefab, meteorPosition, Quaternion.identity);
            meteor.GetComponent<Meteor>().damage = 20f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void SummonMinions()
    {
        bossAnimator.SetBool("SummoningMinions", true);
        StartCoroutine(UseSummonMinions());
        StartCoroutine(ResetAnimatorBool("SummoningMinions"));
    }

    private IEnumerator UseSummonMinions()
    {
        int minionCount = Random.Range(actualStats.minMinionsToSpawn, actualStats.maxMinionsToSpawn + 1);

        for (int i = 0; i < minionCount; i++)
        {
            int spawnPointIndex = Random.Range(0, minionSpawnPoints.Length);
            Transform spawnPoint = minionSpawnPoints[spawnPointIndex];
            Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);

            ParticleSystem particles = Instantiate(minionSpawnParticles, spawnPoint.position, Quaternion.identity);
            particles.Play();

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Whirlwind()
    {
        if (!isWhirlwindActive)
        {
            bossAnimator.SetBool("ActivatingWhirlwind", true);
            isWhirlwindActive = true;

            // Store the original moveSpeed
            float originalMoveSpeed = enemyStats.ActualStats.moveSpeed;

            // Modify the moveSpeed during whirlwind
            enemyStats.ModifyActualStats(new EnemyStats.Stats { moveSpeed = originalMoveSpeed * actualStats.whirlwindMoveSpeedMultiplier });

            StartCoroutine(ActivateWhirlwind());
            StartCoroutine(StopWhirlwindAfterDuration(actualStats.whirlwindDuration, originalMoveSpeed));
        }
    }

    private IEnumerator StopWhirlwindAfterDuration(float duration, float originalMoveSpeed)
    {
        yield return new WaitForSeconds(duration);
        
        // Restore the original moveSpeed
        enemyStats.ModifyActualStats(new EnemyStats.Stats { moveSpeed = originalMoveSpeed + 0.5f});

        isWhirlwindActive = false;
        hasUsedWhirlwind = true;
        bossAnimator.SetBool("ActivatingWhirlwind", false);
    }

    private IEnumerator ActivateWhirlwind()
    {
        float timer = 0f;

        while (timer < actualStats.whirlwindDuration)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, actualStats.whirlwindRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    collider.GetComponent<PlayerStats>().TakeDamage(actualStats.whirlwindDamage);
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ResetAnimatorBool(string boolName)
    {
        yield return new WaitForEndOfFrame();
        bossAnimator.SetBool(boolName, false);
    }

    public void ApplyDifficultyScaling(DifficultyManager.DifficultyStats difficultyStats)
    {
        actualStats += difficultyStats.crimsonCrusaderStats;
    }
}
