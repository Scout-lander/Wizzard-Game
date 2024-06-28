using UnityEngine;

public class IceTrail : MonoBehaviour
{
    public float fadeDuration = 3f; // Duration for fading out

    // Variables for ice slow effect
    private float iceSlowPercentage = .50f;
    private float iceSlowDuration = 3;


    private void Start()
    {
        // Start fading out when instantiated
        StartCoroutine(FadeOutCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if collided with the player
        if (collision.CompareTag("Player"))
        {
            // Apply slow effect to the player
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyIceSlow();
            }

            // Destroy the ice trail prefab upon collision with the player
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator FadeOutCoroutine()
    {
        // Start fading out
        float elapsedTime = 0f;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Fade to transparent

        while (elapsedTime < fadeDuration)
        {
            // Calculate the new color based on the elapsed time
            float t = elapsedTime / fadeDuration;
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, t);

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the sprite is fully transparent
        spriteRenderer.color = targetColor;

        // Destroy the ice trail after fading out
        Destroy(gameObject);
    }
}
