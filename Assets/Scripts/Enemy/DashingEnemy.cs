using System.Collections;
using UnityEngine;

public class DashingEnemy : MonoBehaviour
{
    protected bool canDash = true;
    protected bool isCharging = false;
    protected Coroutine dashCoroutine;
    protected Transform player;    // Reference to the player
    private EnemyStats enemyStats;
    protected  Animator animator;

    // Stats class to manage all stats as a single object.
    [System.Serializable]
    public class Stats
    {
        public float dashDistance = 5f;
        public float dashSpeed = 10f;
        public float dashCooldown = 3f;
        public float stunDuration = 1f;
        

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.dashDistance += s2.dashDistance;
            s1.dashSpeed += s2.dashSpeed;
            s1.dashCooldown = Mathf.Max(0.1f, s1.dashCooldown + s2.dashCooldown);
            s1.stunDuration += s2.stunDuration;
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
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyStats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();

        // Adjust enemy parameters based on difficulty level
        int difficultyLevel = DifficultyManager.Instance.CurrentDifficultyLevel;
        AdjustStatsBasedOnDifficulty(difficultyLevel);

        StartCoroutine(CheckDashConditions());
    }

    private IEnumerator CheckDashConditions()
    {
        while (true)
        {
            // Check if the player is within dash distance
            if (Vector3.Distance(transform.position, player.position) <= actualStats.dashDistance && canDash)
            {
                // Start the dash coroutine
                StartCoroutine(Dash());
            }
            yield return null;
        }
    }

    // Dash coroutine
    protected IEnumerator Dash()
    {
        isCharging = true;
        canDash = false; // Disable dashing until cooldown

        float originalMoveSpeed;
        originalMoveSpeed = enemyStats.Actual.moveSpeed; // Access moveSpeed directly from the instance
        enemyStats.ModifyActualStats(new EnemyStats.Stats { moveSpeed =  0}); // removes move speed

        animator.SetBool("Jump", true);  // trigger dash animation
        yield return new WaitForSeconds(.3f);

        Vector3 targetPosition = player.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distanceRemaining = actualStats.dashDistance;

        while (distanceRemaining > 0)
        {
            // Move towards the player at dash speed
            transform.position += direction * actualStats.dashSpeed * Time.deltaTime;
            distanceRemaining -= actualStats.dashSpeed * Time.deltaTime;

            yield return null;
        }

        animator.SetBool("Jump", false);
        isCharging = false;
        enemyStats.ModifyActualStats(new EnemyStats.Stats { moveSpeed = originalMoveSpeed }); // returns movespeed

        yield return new WaitForSeconds(actualStats.dashCooldown);// Wait for dash cooldown
        canDash = true; // Enable dashing again
    }

    private void AdjustStatsBasedOnDifficulty(int difficultyLevel)
    {
        actualStats.dashDistance += difficultyLevel * 1f; // Increase dash distance with difficulty
        actualStats.dashSpeed += difficultyLevel * 1f; // Increase dash speed with difficulty
        actualStats.dashCooldown = Mathf.Max(0.1f, actualStats.dashCooldown - 0.2f * difficultyLevel); // Reduce cooldown with difficulty
        actualStats.stunDuration += difficultyLevel * 0.5f; // Increase stun duration with difficulty
    }

    public void ApplyDifficultyScaling(DifficultyManager.DifficultyStats difficultyStats)
    {
        actualStats += difficultyStats.dashingEnemyStats;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        //Reference the script from the collided collider and deal damage using TakeDamage()
        if (col.gameObject.CompareTag("Player") && isCharging)
        {
            // Get the PlayerMovement component from the collided player object
            PlayerMovement playerMovement = col.gameObject.GetComponent<PlayerMovement>();

            // Apply stun and knockback to the player
            Vector2 sourcePosition = transform.position; // The position of the enemy causing the stun
            float knockbackForce = 2f; // Adjust as needed
            float knockbackDuration = 0.5f; // Adjust as needed
            
            StartCoroutine(playerMovement.ApplyStun(sourcePosition, knockbackForce, knockbackDuration, actualStats.stunDuration));
        }
    }
}