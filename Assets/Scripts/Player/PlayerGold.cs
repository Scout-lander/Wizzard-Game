using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public static PlayerGold instance;

    public int totalGold;

    public delegate void OnGoldChanged(int newGoldAmount);
    public event OnGoldChanged onGoldChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to add gold to the player's total
    public void AddGold(int amount)
    {
        totalGold += amount;
        onGoldChanged?.Invoke(totalGold);
        Debug.Log("Gold added: " + amount + ". Total gold: " + totalGold);
    }
}
