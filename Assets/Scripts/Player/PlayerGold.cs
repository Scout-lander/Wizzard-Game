using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public static PlayerGold Instance { get; private set; }
    public int gold;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        // Update UI or other systems as needed
    }

    public int GetGold()
    {
        return gold;
    }
}
