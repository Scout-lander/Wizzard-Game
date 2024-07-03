using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public const float DEFAULT_MOVESPEED = 5f;

    // Movement
    public Vector2 moveDir { get; private set; }
    public float lastHorizontalVector { get; private set; }
    public float lastVerticalVector { get; private set; }
    public Vector2 lastMovedVector { get; private set; }
    private bool movementLocked = false;

    // Dashing
    public bool isDashing { get; private set; }
    public float currentDashes { get; private set; }
    private float dashSpeed;
    private bool dashCooldown = false;
    private float SBuff;

    // Stun
    public bool isStunned { get; private set; } = false;
    public bool canBeStunned { get; private set; } = true;
    public ParticleSystem stunParticles;

    // Slowed
    private bool isSlowed;
    public float slowFactor { get; private set; }

    // Ice Slowed
    public bool IsIceSlowed { get; private set; } = false;
    private float IcedSlow;
    private float icedDuration;

    // References
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerStats player;
    private FrostCoolDownUI frostCoolDown;
    private CoolDowns coolDowns;

    void Start()
    {
        player = GetComponent<PlayerStats>();
        coolDowns = FindObjectOfType<CoolDowns>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);
        animator = GetComponent<Animator>();
        dashSpeed = player.ActualStats.moveSpeed * 20;
        SBuff = player.ActualStats.moveSpeed * 20;
        EnemyAbility enemyAbilityInstance = new EnemyAbility();
        IcedSlow = enemyAbilityInstance.iceSlowPercentage;
        icedDuration = enemyAbilityInstance.iceSlowDuration;
        frostCoolDown = FindObjectOfType<FrostCoolDownUI>();
        StartCoroutine(LateStart());
    }

    void Update()
    {
        if (GameManager.instance.isGameOver) return;
        if (isStunned || isDashing) return;

        HandleInput();
    }

    void FixedUpdate()
    {
        if (!isStunned && !isDashing)
            Move();
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        currentDashes = player.ActualStats.maxDashes;
    }

    public PlayerStats GetPlayerStats()
    {
        return player;
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentDashes > 0 && !IsIceSlowed)
        {
            StartCoroutine(Dash());
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);
        }

        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);
        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }

    void Move()
    {
        if (movementLocked) return;

        float currentSpeed = DEFAULT_MOVESPEED * player.ActualStats.moveSpeed;
        if (isSlowed) currentSpeed *= slowFactor;
        if (IsIceSlowed) currentSpeed *= IcedSlow;

        rb.velocity = moveDir * currentSpeed;
    }

    public void DoDash()
    {
        StartCoroutine(Dash());
    }

    public IEnumerator Dash()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        currentDashes--;
        isDashing = true;

        rb.velocity = moveDir * dashSpeed;
        animator.Play("Wizzard_dash");
        yield return new WaitForSeconds(player.ActualStats.dashDuration);
        isDashing = false;
        Physics2D.IgnoreLayerCollision(7, 8, false);
        animator.Play("Wizzard_Idle");

        if (currentDashes < player.ActualStats.maxDashes)
        {
            StartCoroutine(ReplenishDash());
        }
    }

    public IEnumerator ReplenishDash()
    {
        while (currentDashes < player.ActualStats.maxDashes && !dashCooldown)
        {
            dashCooldown = true;
            coolDowns.DashUI();
            yield return new WaitForSeconds(player.ActualStats.dashCooldown);
            currentDashes++;
            dashCooldown = false;
        }
    }

    public void ActiveSpeedBuff()
    {
        rb.velocity = moveDir * player.ActualStats.moveSpeed * SBuff;
    }

    public void DeactiveSpeedBuff()
    {
        rb.velocity = moveDir * player.ActualStats.moveSpeed / SBuff;
    }

    public void LockMovement(bool lockMovement)
    {
        movementLocked = lockMovement;
        rb.constraints = lockMovement ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.FreezeRotation;
    }

    public void Knockback(Vector2 direction, float force)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public IEnumerator ApplyKnockback(Vector2 direction, float force, float duration)
    {
        Knockback(direction, force);
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;
    }

    public IEnumerator ApplyStun(Vector2 sourcePosition, float knockbackForce, float knockbackDuration, float stunDuration)
    {
        if (!canBeStunned) yield break;

        isStunned = true;
        canBeStunned = false;
        player.ApplyMaterial();

        Knockback(((Vector2)transform.position - sourcePosition).normalized * knockbackForce, knockbackDuration);
        yield return new WaitForSeconds(stunDuration * player.ActualStats.StunReduction);

        isStunned = false;
        player.ApplyMaterial();
        yield return new WaitForSeconds(4);
        canBeStunned = true;
    }

    public void ApplyIceSlow()
    {
        IsIceSlowed = true;
        player.ApplyMaterial();
        StartCoroutine(RevertSpeedAfterDelay());
    }

    public void ApplySlow(float slowFactor)
    {
        isSlowed = true;
        this.slowFactor = slowFactor;
    }

    public void RemoveSlow()
    {
        isSlowed = false;
    }

    private IEnumerator RevertSpeedAfterDelay()
    {
        yield return new WaitForSeconds(icedDuration * player.ActualStats.SlowReduction);
        IsIceSlowed = false;
        player.ApplyMaterial();
    }
}
