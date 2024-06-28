using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;

    GameObject player; // Reference to the player GameObject

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find the player GameObject by tag
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found. Make sure the player GameObject is tagged as 'Player'.");
        }
    }

    void Update()
    {
        // Check if the player GameObject exists and flip sprite accordingly
        if (player != null)
        {
            FlipSpriteDirection();
        }
    }

    void FlipSpriteDirection()
    {
        // Get the player's position
        Vector3 playerPosition = player.transform.position;

        // Flip sprite based on player's position relative to the enemy
        if (playerPosition.x < transform.position.x)
        {
            spriteRenderer.flipX = true; // Player is on the left side
        }
        else
        {
            spriteRenderer.flipX = false; // Player is on the right side
        }
    }
}
