using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    [System.Serializable]
    public class Stats
    {
        public float maxHealth = 10, moveSpeed = 1.5f, damage = 3, knockbackMultipler = 1f;

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.moveSpeed += s2.moveSpeed;
            s1.damage += s2.damage;
            s1.knockbackMultipler += s2.knockbackMultipler;
            return s1;
        }
    }

    public EnemyAbility enemyAbility;
    public Stats baseStats;
    [SerializeField] private Stats actualStats;
    private float health;
    private float damageBlockPercentage;
    public bool hasShield;

    public Stats ActualStats
    {
        get { return actualStats; }
    }
    private EnemyGold enemyGold;
    private PlayerGold playerGold;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    private Color originalColor;
    private SpriteRenderer sr;
    private EnemyMovement movement;
    private GameManager gameManager;

    public static int count;

    void Awake()
    {
        count++;
        health = baseStats.maxHealth;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        actualStats = baseStats;
        movement = GetComponent<EnemyMovement>();
        enemyAbility = GetComponent<EnemyAbility>();
        gameManager = FindObjectOfType<GameManager>();

        AdjustStatsBasedOnDifficulty(DifficultyManager.Instance.CurrentDifficultyLevel);

        playerGold = FindObjectOfType<PlayerGold>();
        enemyGold = GetComponent<EnemyGold>();
    }

    public void ModifyActualStats(Stats modification)
    {
        actualStats += modification;
    }

    public void ApplyShieldEffect(float blockPercentage)
    {
        hasShield = true;
        damageBlockPercentage = blockPercentage;
    }

    public void RemoveShieldEffect()
    {
        damageBlockPercentage = 0;
        hasShield = false;
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        float damage = dmg * (1f - damageBlockPercentage);
        health -= damage;
        if (damage > 0)
            StartCoroutine(DamageFlash());

        if (damage > 0)
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);

        if (knockbackForce > 0 && damage > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if (enemyAbility != null && enemyAbility.hasLightningAbility && enemyAbility.lightningProjectilePrefab != null)
        {
            Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
            enemyAbility.InstantiateLightningProjectile(transform.position, direction);
        }

        gameManager.IncrementTotalDamageDone(damage);

        if (health <= 0)
        {
            Kill();
        }
    }

    private IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        if (GetComponent<SplittingEnemy>() != null)
        {
            SplittingEnemy splittingEnemy = GetComponent<SplittingEnemy>();
            splittingEnemy.OnKill();
        }

        if (enemyAbility != null && enemyAbility.hasIceAbility)
        {
            enemyAbility.SpawnIceSpike();
        }

        if (enemyAbility != null && enemyAbility.leavesPoisonCloud)
        {
            enemyAbility.CreatePoisonCloud();
        }

        StartCoroutine(KillFade());
        gameManager.IncrementKillCount();
        enemyGold.DropGold();
    }

    private IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(actualStats.damage);
        }
    }

    private void OnDestroy()
    {
        count--;
    }

    private void AdjustStatsBasedOnDifficulty(int difficultyLevel)
    {
        actualStats.maxHealth += 2 * difficultyLevel;
        actualStats.moveSpeed += 0.1f * difficultyLevel;
        actualStats.damage += 1 * difficultyLevel;
        actualStats.knockbackMultipler += 0.1f * difficultyLevel;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log($"{gameObject.name} collided with {col.gameObject.name} on Enter.");
    }
}
