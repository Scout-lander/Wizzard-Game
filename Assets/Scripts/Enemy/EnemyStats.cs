using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    // Stats class to allow managing of all stats as a single object.
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

    // Storing stats as baseStats and actualStats allow us to implement buffs / debuffs in future.
    public Stats baseStats;
    [SerializeField] Stats actualStats;
    float health;
    private float damageBlockPercentage; // Percentage of damage to block (0.0f - 1.0f)
    public bool hasSheild;

    public Stats ActualStats
    {
        get { return actualStats; }
    }

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1); // What the color of the damage flash should be.
    public float damageFlashDuration = 0.2f; // How long the flash should last.
    public float deathFadeTime = 0.6f; // How much time it takes for the enemy to fade.
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;
    GameManager gameManager;

    public static int count; // Track the number of enemies on the screen.


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
    }

    public void ModifyActualStats(Stats modification)
    {
        actualStats = modification;
    }

    public void ApplyShieldEffect(float BlockPercentage)
    {
        hasSheild = true;
        float originalBlock = damageBlockPercentage;
        damageBlockPercentage += 0;  // Increase damage block
    }

    public void RemoveSheildEffect() //Debug.Log("Shield is touching an enemy.");
    {
        damageBlockPercentage = 0;  // Revert to 0 damage block
        hasSheild = false;

    }

    // This function always needs at least 2 values, the amount of damage dealt <dmg>, as well as where the damage is
    // coming from, which is passed as <sourcePosition>. The <sourcePosition> is necessary because it is used to calculate
    // the direction of the knockback.
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        // Apply damage block percentage
        float damage = dmg * (1f - damageBlockPercentage);
    
        health -= damage;
        if (damage > 0)
        StartCoroutine(DamageFlash());

        // Create the text popup when enemy takes damage.
        if (damage > 0)
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);

        // Apply knockback if it is not zero.
        if(knockbackForce > 0 && damage > 0)
        {
            // Gets the direction of knockback.
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

         // Check if the enemyAbility reference is not null and it has the lightning ability and a valid lightning projectile prefab
        if (enemyAbility != null && enemyAbility.hasLightningAbility && enemyAbility.lightningProjectilePrefab != null)
        {
            // Calculate direction from the enemy's position to the source position
            Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;

            // Call the InstantiateLightningProjectile method from the enemyAbility reference
            enemyAbility.InstantiateLightningProjectile(transform.position, direction);
        }

        // Update total damage done
        gameManager.IncrementTotalDamageDone(damage);

        // Kills the enemy if the health drops below zero.
        if (health <= 0)
        {
            Kill();
        }
    }

    // This is a Coroutine function that makes the enemy flash when taking damage.
    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        // If the enemy is a splitting enemy, call its OnKill() method
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

    }

    // This is a Coroutine function that fades the enemy away slowly.
    IEnumerator KillFade()
    {
        // Waits for a single frame.
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        // This is a loop that fires every frame.
        while(t < deathFadeTime) {
            yield return w;
            t += Time.deltaTime;

            // Set the colour for this frame.
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
    }


    void OnCollisionStay2D(Collision2D col)
    {
        //Reference the script from the collided collider and deal damage using TakeDamage()
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(actualStats.damage); // Make sure to use currentDamage instead of weaponData.Damage in case any damage multipliers in the future
        }
    }

    private void OnDestroy()
    {
        count--;
    }

    // Method to adjust stats based on difficulty level
    private void AdjustStatsBasedOnDifficulty(int difficultyLevel)
    {
        // Example adjustments (you can modify as needed)
        actualStats.maxHealth += 2 * difficultyLevel;
        actualStats.moveSpeed += 0.1f * difficultyLevel;
        actualStats.damage += 1 * difficultyLevel;
        actualStats.knockbackMultipler += 0.1f * difficultyLevel;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log($"{gameObject.name} collided with {col.gameObject.name} on Enter.");
    }
}
