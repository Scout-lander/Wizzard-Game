using System.Collections;
using UnityEngine;

public class ShootingEnemy : MonoBehaviour 
{
[System.Serializable]
public class Stats
{
    public float shootingDamage = 10f, shootingDistance = 6f, projectileSpeed = 7, shootingCooldown = 1f;

    public static Stats operator +(Stats s1, Stats s2)
        {
            s1.shootingDamage += s2.shootingDamage;
            s1.shootingDistance += s2.shootingDistance;
            s1.projectileSpeed += s2.projectileSpeed;
            s1.shootingCooldown += s2.shootingCooldown;
            return s1;
        }
}
    public GameObject projectilePrefab; // Reference to the projectile prefab
    protected Transform player;    // Reference to the player

    public Stats baseStats;
    [SerializeField] Stats actualStats;
    public Stats ActualStats
    {
        get { return actualStats; }
    }


    // Internal state variables
    protected bool canShoot = true;
    protected Coroutine shootingCoroutine;

    protected void Start()
    {
        actualStats = baseStats;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartShooting();
        //ApplyDifficultyScaling(DifficultyManager.Instance.GetCurrentDifficultyStats());
    }

    protected void StartShooting()
    {
        // Start the shooting coroutine
        shootingCoroutine = StartCoroutine(ShootCoroutine());
    }

    protected void StopShooting()
    {
        // Stop the shooting coroutine
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    protected IEnumerator ShootCoroutine()
    {
        // Shooting coroutine logic
        while (true)
        {
            if (player != null && canShoot && Vector2.Distance(transform.position, player.position) <= actualStats.shootingDistance)
            {
                Shoot();
            }
            yield return new WaitForSeconds(actualStats.shootingCooldown);
        }
    }

    protected void Shoot()
    {
        // Instantiate and shoot projectile towards the player
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector3 direction = (player.position - transform.position).normalized;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * actualStats.projectileSpeed;

        // Optionally, you can set additional properties of the projectile, such as damage
        ProjectileScript projectileScript = projectile.GetComponent<ProjectileScript>();
        if (projectileScript != null)
        {
            projectileScript.SetDamage((int)actualStats.shootingDamage); // Casting shootingDamage to int
        }

        // Reset shooting cooldown
        canShoot = false;
        StartCoroutine(ResetShoot());
    }

    protected IEnumerator ResetShoot()
    {
        // Reset shooting cooldown
        yield return new WaitForSeconds(actualStats.shootingCooldown);
        canShoot = true;
    }

    private void AdjustStatsBasedOnDifficulty(int difficultyLevel)
    {
        // Example adjustments (you can modify as needed)
        actualStats.shootingDamage += 2 * difficultyLevel;
        actualStats.shootingDistance += 1 * difficultyLevel;
        actualStats.projectileSpeed += 1 * difficultyLevel;
        actualStats.shootingCooldown -= 0.1f * difficultyLevel;
    }

    public void ApplyDifficultyScaling(DifficultyManager.DifficultyStats difficultyStats)
    {
        actualStats += difficultyStats.shootingEnemyStats;
    }
}
