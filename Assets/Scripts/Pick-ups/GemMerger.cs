using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMerger : MonoBehaviour
{
    public GameObject superGemPrefab;
    public KeyCode mergeKey = KeyCode.M;
    public int autoMergeAmount = 100;
    public float autoMergeCooldown = 5f; // Cooldown duration in seconds
    public List<Transform> spawnPoints; // List of spawn points
    public BoxCollider2D mergeArea; // 2D box collider defining the merging area

    private float lastAutoMergeTime;
    private Coroutine autoMergeCoroutine;

    [SerializeField]
    private int experienceGemsOutsideMergeAreaCount;

    public int ExperienceGemsOutsideMergeAreaCount
    {
        get { return experienceGemsOutsideMergeAreaCount; }
        private set { experienceGemsOutsideMergeAreaCount = value; }
    }

    void Start()
    {
        // Start the auto merge coroutine
        autoMergeCoroutine = StartCoroutine(AutoMergeRoutine());
    }

    void Update()
    {
        // Check if the merge key is pressed
        if (Input.GetKeyDown(mergeKey))
        {
            MergeExperienceGems();
        }
    }

    IEnumerator AutoMergeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoMergeCooldown - 1f); // Wait until 1 second before the cooldown ends
            ExperienceGemsOutsideMergeAreaCount = CountExperienceGemsOutsideMergeArea();

            if (ExperienceGemsOutsideMergeAreaCount >= autoMergeAmount)
            {
                yield return new WaitForSeconds(1f); // Wait for the remaining 1 second of the cooldown
                MergeExperienceGems();
                lastAutoMergeTime = Time.time; // Reset the cooldown timer
            }
        }
    }

    void MergeExperienceGems()
    {
        Pickup[] allPickups = FindObjectsOfType<Pickup>();
        List<Pickup> experienceGems = new List<Pickup>();
        int totalExperience = 0;

        // Collect all experience gems outside the merge area
        foreach (var pickup in allPickups)
        {
            if (pickup.pickupType == PickupType.Experience && !IsInsideMergeArea(pickup.transform.position))
            {
                experienceGems.Add(pickup);
                totalExperience += pickup.experience;
            }
        }

        // Destroy all collected experience gems and update the pickup manager count
        foreach (var gem in experienceGems)
        {
            Destroy(gem.gameObject);
            PickupManager.Instance.RemovePickup(PickupType.Experience);
        }

        // Instantiate the super gem at a random spawn point
        if (experienceGems.Count > 0 && spawnPoints.Count > 0)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject superGem = Instantiate(superGemPrefab, randomSpawnPoint.position, Quaternion.identity);

            // Set the combined XP on the super gem
            Pickup superGemPickup = superGem.GetComponent<Pickup>();
            superGemPickup.experience = totalExperience;

            // Update the pickup manager with the new super gem
            PickupManager.Instance.AddPickup(PickupType.SuperGem);
        }
    }

    int CountExperienceGemsOutsideMergeArea()
    {
        Pickup[] allPickups = FindObjectsOfType<Pickup>();
        int count = 0;

        foreach (var pickup in allPickups)
        {
            if (pickup.pickupType == PickupType.Experience && !IsInsideMergeArea(pickup.transform.position))
            {
                count++;
            }
        }

        return count;
    }

    bool IsInsideMergeArea(Vector3 position)
    {
        if (mergeArea == null)
        {
            Debug.LogError("Merge area not set!");
            return false;
        }

        return mergeArea.bounds.Contains(position);
    }
}
