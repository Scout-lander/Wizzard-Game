using UnityEngine;

public class VoxLaunch : MonoBehaviour
{
    public void Launch(GameObject voxPrefab, Vector3 targetPosition, float effectRadius, float pullForce, float slowFactor, float effectDuration)
    {
        // Calculate direction towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Instantiate Vox prefab at the target position
        GameObject voxInstance = Instantiate(voxPrefab, targetPosition, Quaternion.identity);

        // Initialize the VoxEffect script with parameters
        VoxEffect voxEffect = voxInstance.GetComponent<VoxEffect>();
        if (voxEffect != null)
        {
            voxEffect.Initialize(effectRadius, pullForce, slowFactor, effectDuration);
        }
        else
        {
            Debug.LogWarning("VoxEffect script not found on Vox prefab.");
        }

        // Optionally, you could add more logic here, like applying forces or animations
    }
}
