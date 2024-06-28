using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public const float DEFAULT_MOVESPEED = 5f;

    // Movement
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMovedVector;
    public float moveSpeed = 5;

    // Dashing
    public bool isDashing;
    public float currentDashes = 1;
    public float maxDashes = 3; // Maximum number of dashes
    private float dashSpeed;
    private bool dashCooldown = false;

    // References
    Animator am;
    Rigidbody2D rb;
    SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        lastMovedVector = new Vector2(1, 0f);
        dashSpeed = moveSpeed * 20;
    }

    void Update()
    {
        InputManagement();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        if (isDashing)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && currentDashes > 0)
        {
            StartCoroutine(Dash());
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f); // Last moved X
        }

        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector); // Last moved Y
        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector); // While moving
        }
    }

    void Move()
    {
        if (isDashing)
        {
            return;
        }

        rb.velocity = moveDir * DEFAULT_MOVESPEED * moveSpeed;
    }

    public IEnumerator Dash()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        currentDashes--; // Use one dash
        isDashing = true;

        rb.velocity = new Vector2(moveDir.x * dashSpeed, moveDir.y * dashSpeed);
        am.Play("Wizzard_dash");
        yield return new WaitForSeconds(.3f);
        isDashing = false;
        am.Play("Wizzard_Idle");

        // Start the cooldown for replenishing dashes if not already started
        if (!dashCooldown)
        {
            StartCoroutine(ReplenishDash());
        }

        Physics2D.IgnoreLayerCollision(7, 8, false);
    }

    public IEnumerator ReplenishDash()
    {
        dashCooldown = true;
        while (currentDashes < maxDashes)
        {
            yield return new WaitForSeconds(1); // Wait for 1 second before replenishing
            currentDashes++;
        }
        dashCooldown = false;
    }

    void UpdateAnimation()
    {
        if (moveDir.x != 0 || moveDir.y != 0)
        {
            am.SetBool("Move", true);
            SpriteDirectionChecker();
        }
        else
        {
            am.SetBool("Move", false);
        }
    }

    void SpriteDirectionChecker()
    {
        if (lastHorizontalVector < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }
}
