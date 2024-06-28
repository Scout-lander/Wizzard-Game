using UnityEngine;

public class PortalAnimator : MonoBehaviour
{
    public ParticleSystem portalEffect; // Reference to the ParticleSystem

    private void Start()
    {
        if (portalEffect != null)
        {
            portalEffect.Stop(); // Ensure the particle effect is stopped initially
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (portalEffect != null && !portalEffect.isPlaying)
            {
                portalEffect.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (portalEffect != null && portalEffect.isPlaying)
            {
                portalEffect.Stop();
            }
        }
    }
}
