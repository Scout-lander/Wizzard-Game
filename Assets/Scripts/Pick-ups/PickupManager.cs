using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager Instance;

    [SerializeField] public int healthPickupCount;
    [SerializeField] public int experiencePickupCount;
    [SerializeField] public  int magnetPickupCount;
    [SerializeField] public int superGemCount; // Count for super gems

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPickup(PickupType type)
    {
        switch (type)
        {
            case PickupType.Health:
                healthPickupCount++;
                break;
            case PickupType.Experience:
                experiencePickupCount++;
                break;
            case PickupType.Magnet:
                magnetPickupCount++;
                break;
            case PickupType.SuperGem: // Increment super gem count
                superGemCount++;
                break;
        }
        //Debug.Log($"Health Pickups: {healthPickupCount}, Experience Pickups: {experiencePickupCount}, Magnet Pickups: {magnetPickupCount}, Super Gems: {superGemCount}");
    }

    public void RemovePickup(PickupType type)
    {
        switch (type)
        {
            case PickupType.Health:
                healthPickupCount--;
                break;
            case PickupType.Experience:
                experiencePickupCount--;
                break;
            case PickupType.Magnet:
                magnetPickupCount--;
                break;
            case PickupType.SuperGem: // Decrement super gem count
                superGemCount--;
                break;
        }
        //Debug.Log($"Health Pickups: {healthPickupCount}, Experience Pickups: {experiencePickupCount}, Magnet Pickups: {magnetPickupCount}, Super Gems: {superGemCount}");
    }
}
