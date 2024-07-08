using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    [System.Serializable]
    public class Stats
    {
        public float maxHealth = 10, moveSpeed = 1.5f, damage = 3, knockbackMultiplier = 1f;

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.moveSpeed += s2.moveSpeed;
            s1.damage += s2.damage;
            s1.knockbackMultiplier += s2.knockbackMultiplier;
            return s1;
        }
    }

    public EnemyAbility enemyAbility;
    public Stats baseStats;
    [SerializeField] private Stats actualStats;
    private float health;
    public float damageBlockPercentage;
    public bool hasShield;
    public int goldAmount = 10;

    public Stats ActualStats => actualStats;

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
        sr = GetComponent<SpriteRenderer>();
        movement = GetComponent<EnemyMovement>();
        enemyAbility = GetComponent<EnemyAbility>();
        gameManager = FindObjectOfType<GameManager>();

        originalColor = sr.color;
        actualStats = baseStats;

        AdjustStatsBasedOnDifficulty(DifficultyManager.Instance?.CurrentDifficultyLevel ?? 0);
        health = actualStats.maxHealth;
    }

    public void ModifyActualStats(Stats modification)
    {
        actualStats += modification;
    }

    public void ApplyShieldEffect(float blockPercentage)
    {
        hasShield = true;
        damageBlockPercentage = 10;
    }

    public void RemoveShieldEffect()
    {
        hasShield = false;
        damageBlockPercentage = 0;
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        float damage = dmg * (1f - damageBlockPercentage);
        health -= damage;

        if (damage > 0)
        {
            StartCoroutine(FlashDamage());
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
            gameManager?.IncrementTotalDamageDone(damage);
        }

        if (knockbackForce > 0 && damage > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        enemyAbility?.InstantiateLightningProjectile(transform.position, ((Vector2)transform.position - sourcePosition).normalized);

        if (health <= 0)
        {
            Kill();
        }
    }

    private IEnumerator FlashDamage()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        GetComponent<SplittingEnemy>()?.OnKill();
        enemyAbility?.SpawnIceSpike();
        enemyAbility?.CreatePoisonCloud();

        StartCoroutine(KillFade());
        GameManager.instance.IncrementGold(goldAmount);
        gameManager?.IncrementKillCount();
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
            col.gameObject.GetComponent<PlayerStats>()?.TakeDamage(actualStats.damage);
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
        actualStats.knockbackMultiplier += 0.1f * difficultyLevel;
    }
}
