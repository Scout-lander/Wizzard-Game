using UnityEngine;

public class VoxEffect : MonoBehaviour
{
    private float effectRadius;
    private float pullForce;
    private float slowFactor;
    private float effectDuration;

    private Rigidbody2D playerRb;

    private void Start()
    {
        // Destroy the Vox effect after the specified duration
        Destroy(gameObject, effectDuration);
    }

    private void FixedUpdate()
    {
        if (playerRb != null)
        {
            // Calculate direction towards the center of the VoxEffect
            Vector2 direction = (Vector2)transform.position - playerRb.position;
            //Debug.Log("Direction towards center: " + direction);

            // Apply pulling force towards the center
            playerRb.AddForce(direction.normalized * pullForce, ForceMode2D.Force);
            //Debug.Log("Pulling force applied: " + (direction.normalized * pullForce));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Apply slow effect
                playerMovement.ApplySlow(slowFactor);

                // Store player's Rigidbody2D for continuous pulling
                playerRb = other.GetComponent<Rigidbody2D>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Remove slow effect
                playerMovement.RemoveSlow();

                // Clear player's Rigidbody2D reference
                playerRb = null;
            }
        }
    }

    public void Initialize(float radius, float force, float slow, float duration)
    {
        effectRadius = radius;
        pullForce = force;
        slowFactor = slow;
        effectDuration = duration;

        // Adjust the radius of the CircleCollider2D
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = effectRadius;
        }
    }
}
