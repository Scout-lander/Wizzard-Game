using System.Collections;
using UnityEngine;

public class BombEnemy : MonoBehaviour
{
    protected bool ExplodingEnemy = true;
    private bool isExploding = false;
    public Color flashColor = new Color(1, 0, 0, 1);
    public ParticleSystem explodingEffect;

    //Stats class to allow managing of all stats as a single object.
    [System.Serializable]
    public class Stats
    {
        public float movementSpeedIncrease = 3f, flashDuration = 0.2f, explosionDuration = 4f;
        public float explosionRadius, explosionStartDistance, explosionDamage;

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.movementSpeedIncrease += s2.movementSpeedIncrease;
            s1.flashDuration += s2.flashDuration;
            s1.explosionDuration += s2.explosionDuration;
            s1.explosionRadius += s2.explosionRadius;
            s1.explosionStartDistance += s2.explosionStartDistance;
            s1.explosionDamage += s2.explosionDamage;
            return s1;
        }
    }

    // Storing stats as baseStats and actualStats allow us to implement buffs / debuffs in future.
    public Stats baseStats;
    [SerializeField] Stats actualStats;

    public Stats ActualStats
    {
        get { return actualStats; }
    }

    private Color originalColor;
    private SpriteRenderer sr;
    private Transform player;
    private EnemyStats enemystats;

    private int difficultyLevel;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        actualStats = baseStats;
        enemystats = GetComponent<EnemyStats>();
        StartCoroutine(ExplodeCoroutine());
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Adjust enemy parameters based on difficulty level
        AdjustDifficultyLevel(DifficultyManager.Instance.CurrentDifficultyLevel);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with player
        if (collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    private IEnumerator ExplodeCoroutine()
    {
        while (true)
        {
            yield return null;

            // Check if the player reference is assigned and within the explosion start distance
            if (player != null && Vector2.Distance(transform.position, player.position) <= actualStats.explosionStartDistance)
            {
                isExploding = true;

                // Increase movement speed and move pattern
               // enemystats.ModifyActualStats(new EnemyStats.Stats { moveSpeed = actualStats.movementSpeedIncrease });

                // Flash color
                StartCoroutine(FlashColor());

                // Wait for flash duration
                yield return new WaitForSeconds(actualStats.explosionDuration);

                // Explode
                Explode();

                // Destroy the enemy
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator FlashColor()
    {
        float elapsedTime = 0f;

        while (elapsedTime < actualStats.flashDuration)
        {
            sr.color = (elapsedTime % 0.5f < 0.25f) ? flashColor : originalColor;
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        sr.color = originalColor;
    }

    private void Explode()
    {
        if (explodingEffect != null)
        {
            Instantiate(explodingEffect, transform.position, Quaternion.identity);
        }

        // Deal damage to nearby objects within explosion radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, actualStats.explosionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerStats>().TakeDamage(actualStats.explosionDamage);
            }
            else if (collider.CompareTag("Enemy"))
            {
                Destroy(collider.gameObject); // Destroy the enemy
            }
        }

        // Destroy the bomb enemy
        Destroy(gameObject);
    }

    public void ApplyDifficultyScaling(DifficultyManager.DifficultyStats difficultyStats)
    {
        actualStats += difficultyStats.bombEnemyStats;
    }

    public void AdjustDifficultyLevel(int difficultyLevel)
    {
        this.difficultyLevel = difficultyLevel;

        // You can customize these values based on your difficulty scaling strategy
        actualStats.explosionRadius = baseStats.explosionRadius * (1 + difficultyLevel * 0.2f);
        actualStats.explosionStartDistance = baseStats.explosionStartDistance * (1 + difficultyLevel * 0.1f);
        actualStats.explosionDamage = baseStats.explosionDamage * (1 + difficultyLevel * 0.3f);
    }
}
