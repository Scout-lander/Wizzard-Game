using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Terresquall;

public class PlayerMovement : MonoBehaviour
{

    public const float DEFAULT_MOVESPEED = 5f;

    //Movement
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMovedVector;
    private bool movementLocked = false;
    
    // Dashing
    public bool isDashing;
    public float currentDashes;
    private float dashSpeed;
    private bool dashCooldown = false;
    private float SBuff;

    // Stun
    public bool isStunned = false;
    public bool canBeStunned = true;
    private float stunned = 3;
    public ParticleSystem stunParticles; // Reference to the particle system

    //Slowed
    private bool isSlowed;
    public float slowFactor;

    // Ice Slowed
    public bool IsIceSlowed = false;
    private float IcedSlow;
    private float icedDuration;

    //References
    UnityEngine.Animation anim;
    Animator am;
    Rigidbody2D rb;
    public PlayerStats player;
    FrostCoolDownUI frostCoolDown;
    CoolDowns coolDowns;


    void Start()
    {
    player = GetComponent<PlayerStats>();
    coolDowns = FindObjectOfType<CoolDowns>();
    rb = GetComponent<Rigidbody2D>();
    lastMovedVector = new Vector2(1, 0f);
    anim = GetComponent<UnityEngine.Animation>();
    //currentDashes = player.Stats.maxDashes;
    am = GetComponent<Animator>();
    dashSpeed = player.Stats.moveSpeed * 20;
    SBuff = player.Stats.moveSpeed * 20;
    EnemyAbility enemyAbilityInstance = new EnemyAbility();
    IcedSlow = enemyAbilityInstance.iceSlowPercentage; 
    icedDuration = enemyAbilityInstance.iceSlowDuration;
    frostCoolDown = FindObjectOfType<FrostCoolDownUI>();
    StartCoroutine(LateStart());    

    }

    void Update()
    {
        InputManagement();
    }
    
    void FixedUpdate()
    {
        if (!isStunned && !isDashing)
            Move();
    }

     IEnumerator LateStart()
    {
        // Wait for end of frame to ensure all Start methods are called
        yield return new WaitForEndOfFrame();
        currentDashes = player.Stats.maxDashes;
    }
    public PlayerStats GetPlayerStats()
    {
        return player;
    }
    void InputManagement()
    {
        if(GameManager.instance.isGameOver)
        {
            return;
        }

        if (isStunned || isDashing)
            return;


        if (Input.GetKeyDown(KeyCode.Space) && currentDashes > 0 && !IsIceSlowed)
        {
            StartCoroutine(Dash());
            am.Play("Wizzard_dash");
        }
        

        float moveX, moveY;
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
        }

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);    //Last moved X

        }

        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);  //Last moved Y

        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);    //While moving

        }
    }

    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        if(isDashing || isStunned)
        {
            return;
        }

        if (isSlowed)
        {
            rb.velocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed * slowFactor;
            return;
        }

        if(IsIceSlowed)
        {
            rb.velocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed * IcedSlow;
            return;
        }

        if(isStunned)
        {
            rb.velocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed * stunned;
            return;
        }

        rb.velocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
    }
    
    public void DoDash()
    {
        StartCoroutine(Dash());
    }

    public IEnumerator Dash()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        currentDashes--; // Use one dash
        isDashing = true;

        rb.velocity = new Vector2(moveDir.x * dashSpeed, moveDir.y * dashSpeed);
        am.Play("Wizzard_dash");
        yield return new WaitForSeconds(player.Stats.dashDuration - 1);
        isDashing = false;
        Physics2D.IgnoreLayerCollision(7, 8, false);
        am.Play("Wizzard_Idle");

        // Start the cooldown for replenishing dashes if not already started
        if (currentDashes < player.Stats.maxDashes)
        {
            StartCoroutine(ReplenishDash());
        }
    }

    public IEnumerator ReplenishDash()
    {
        while (currentDashes < player.Stats.maxDashes && !dashCooldown)
        {
            dashCooldown = true;
            coolDowns.DashUI();
            yield return new WaitForSeconds(player.Stats.dashCooldown - .4f);
            currentDashes++;
            dashCooldown = false;
        }
    }

    public void ActiveSpeedBuff()
    {
        rb.velocity = new Vector2(moveDir.x * player.Stats.moveSpeed * SBuff, moveDir.y * player.Stats.moveSpeed * SBuff);
    }

    public void DeactiveSpeedBuff()
    {
        rb.velocity = new Vector2(moveDir.x * player.Stats.moveSpeed / SBuff, moveDir.y * player.Stats.moveSpeed / SBuff);
    }

    public void LockMovement(bool lockMovement)
    {
        movementLocked = lockMovement;
        if (lockMovement)
        {
            // Freeze movement along all axes (X, Y, and rotation around the Z-axis)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            // Unfreeze movement along all axes
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void Knockback(Vector2 direction, float force)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public IEnumerator ApplyKnockback(Vector2 direction, float force, float duration)
    {
        Knockback(direction, force); // Apply knockback
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero; // Reset velocity after duration
    }

    public IEnumerator ApplyStun(Vector2 sourcePosition, float knockbackForce, float knockbackDuration, float stunDuration)
    {
        if (canBeStunned)
        {
            isStunned = true;
            canBeStunned = false;

            player.ApplyMaterial();

            // Activate stun particles
            //Knockback(((Vector2)transform.position - sourcePosition).normalized * knockbackForce, 0.3f);
            //yield return new WaitForSeconds(0.25f);

            //LockMovement(true);
            //yield return new WaitForSeconds(3f * player.Stats.StunReduction); //How Long the stun lasts

            //am.SetBool("Stunned", false);
            //am.Play("Wizzard_Idle");
            isStunned = false;
            //LockMovement(false);
            player.ApplyMaterial();

            yield return new WaitForSeconds(4); //How long until you can be Stunned again
            canBeStunned = true;
        }
    }

    public void ApplyIceSlow()
    {
        IsIceSlowed = true;
        player.ApplyMaterial();
        if (IsIceSlowed)
        {
            //frostCoolDown.IceSlowed();
            StartCoroutine(RevertSpeedAfterDelay());
        }
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
        yield return new WaitForSeconds(icedDuration * player.Stats.SlowReduction);

        // Revert the speed back to normal
        IsIceSlowed = false;
        player.ApplyMaterial();
    }
}