using UnityEngine;

public class LightningProjectile : MonoBehaviour
{
    public int damageAmount = 10; // Adjust damage as needed

    // Called when this object collides with another 2D collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply damage to the player
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageAmount);
            }
            else
            {
                Debug.LogWarning("PlayerStats component not found on collided object.");
            }

            // Destroy the lightning projectile
            Destroy(gameObject);
        }
    }
}
