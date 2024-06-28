using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    public float duration = 5f; // Duration of the poison cloud
    public float damagePerTick = 1.5f; // Damage inflicted per tick

    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // Check if the poison cloud has exceeded its duration
        if (Time.time - startTime >= duration)
        {
            Destroy(gameObject); // Destroy the poison cloud when its duration is over
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is the player
        if (other.CompareTag("Player"))
        {
            // Apply poison damage to the player
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null && !player.isPoisoned)
            {
                Debug.Log("Player poisoned by poison cloud.");
                player.ApplyPoison(duration, damagePerTick);
            }
        }
    }
}
