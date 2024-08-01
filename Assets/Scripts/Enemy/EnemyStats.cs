using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    [System.Serializable]
    public class Resistances
    {
        [Range(0f, 1f)] public float freeze = 0f, kill = 0f, debuff = 0f;

        // To allow us to multiply the resistances.
        public static Resistances operator *(Resistances r, float factor)
        {
            r.freeze = Mathf.Min(1, r.freeze * factor);
            r.kill = Mathf.Min(1, r.kill * factor);
            r.debuff = Mathf.Min(1, r.debuff * factor);
            return r;
        }
    }

    [System.Serializable]
    public class Stats
    {
        [Min(0)] public float maxHealth, moveSpeed, damage;
        public float knockbackMultiplier;
        public Resistances resistances;

        [System.Flags]
        public enum Boostable { health = 1, moveSpeed = 2, damage = 4, knockbackMultiplier = 8, resistances = 16 }
        public Boostable curseBoosts, levelBoosts;

        private static Stats Boost(Stats s1, float factor, Boostable boostable)
        {
            if ((boostable & Boostable.health) != 0) s1.maxHealth *= factor;
            if ((boostable & Boostable.moveSpeed) != 0) s1.moveSpeed *= factor;
            if ((boostable & Boostable.damage) != 0) s1.damage *= factor;
            if ((boostable & Boostable.knockbackMultiplier) != 0) s1.knockbackMultiplier /= factor;
            if ((boostable & Boostable.resistances) != 0) s1.resistances *= factor;
            return s1;
        }
        // Use the multiply operator for curse.
        public static Stats operator *(Stats s1, float factor) { return Boost(s1, factor, s1.curseBoosts); }

        // Use the XOR operator for level boosted stats.
        public static Stats operator ^(Stats s1, float factor) { return Boost(s1, factor, s1.levelBoosts); }
    }

    public Stats baseStats = new Stats { maxHealth = 10, moveSpeed = 1, damage = 3, knockbackMultiplier = 1 };
    public Stats actualStats;
    public Stats Actual
    {
        get { return actualStats; }
    }
    float currentHealth;

    public EnemyAbility enemyAbility;
    //public Stats baseStats;
    //[SerializeField] private Stats actualStats;
    public float damageBlockPercentage;
    public bool hasShield;
    public int goldAmount = 10;

    //public Stats ActualStats => actualStats;

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
    }

    void Start()
    {
        RecalculateStats();
        currentHealth = actualStats.maxHealth;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        enemyAbility = GetComponent<EnemyAbility>();
        gameManager = FindObjectOfType<GameManager>();

        movement = GetComponent<EnemyMovement>();

        AdjustStatsBasedOnDifficulty(DifficultyManager.Instance?.CurrentDifficultyLevel ?? 0);
    }

    public void ModifyActualStats(Stats modification)
    {
        //actualStats += modification;
    }

    // Calculates the actual stats of the enemy based on a variety of factors.
    public void RecalculateStats()
    {
        // Calculate curse boosts.
        float curse = GameManager.GetCumulativeCurse(),
              level = GameManager.GetCumulativeLevels();
        actualStats = (baseStats * curse) ^ level;
    }
    public void ApplyShieldEffect(float blockPercentage)
    {
        hasShield = true;
        damageBlockPercentage = .7f;
    }

    public void RemoveShieldEffect()
    {
        hasShield = false;
        damageBlockPercentage = 0;
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {

        // If damage is exactly equal to maximum health, we assume it is an insta-kill and 
        // check for the kill resistance to see if we can dodge this damage.
        if(Mathf.Approximately(dmg, actualStats.maxHealth))
        {
            // Roll a die to check if we can dodge the damage.
            // Gets a random value between 0 to 1, and if the number is 
            // below the kill resistance, then we avoid getting killed.
            if(Random.value < actualStats.resistances.kill)
            {
                return; // Don't take damage.
            }
        }

        float damage = enemyAbility.hasShieldAbility ? dmg : dmg * (1f - damageBlockPercentage);
        currentHealth -= damage;

        if (damage > 0)
        {
            StartCoroutine(FlashDamage());
            GameManager.GenerateFloatingText(Mathf.FloorToInt(damage).ToString(), transform);
            gameManager?.IncrementTotalDamageDone(damage);

            if(enemyAbility.hasLightningAbility)
            {
                enemyAbility?.InstantiateLightningProjectile(transform.position, ((Vector2)transform.position - sourcePosition).normalized);
            }        

        }

        if (knockbackForce > 0 && damage > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if (currentHealth  <= 0)
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
        // Check for whether there is a PlayerStats object we can damage.
        if(col.collider.TryGetComponent(out PlayerStats p))
        {
            p.TakeDamage(Actual.damage);
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
