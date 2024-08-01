using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnDistance = 10f;
    private float cooldownTime = 2f; // 2 seconds cooldown
    private float nextSpawnTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && Time.time >= nextSpawnTime)
        {
            SpawnPrefab();
            nextSpawnTime = Time.time + cooldownTime; // Set the next spawn time
        }
    }

    void SpawnPrefab()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Prefab to spawn is not set.");
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera not found.");
            return;
        }

        // Calculate the spawn position just outside of the camera's view
        Vector3 spawnPosition = mainCamera.transform.position + mainCamera.transform.forward * spawnDistance;
        // Optionally, you can adjust the spawn position to be out of view to the left or right of the camera
        spawnPosition += mainCamera.transform.right * (spawnDistance / 2);

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
