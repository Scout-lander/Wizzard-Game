using UnityEngine;

public class IceSpike : MonoBehaviour
{
    public float iceSpikeDamage = 10f; // Damage inflicted by the ice spike
    private float iceSlowDuration = 3;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                // Apply damage to the player
                player.TakeDamage(iceSpikeDamage);
            }

            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyIceSlow();
            }
            // Destroy the ice spike after the collision
            Destroy(gameObject);
        }
    }
}
