using System.Collections.Generic;
using UnityEngine;

public class GemInventoryManager : MonoBehaviour
{
    public static GemInventoryManager Instance { get; private set; }

    [SerializeField] private GemBag gemBag;
    [SerializeField] private GemBag equippedBag;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public GemBag GetGemBag()
    {
        return gemBag;
    }

    public GemBag GetEquippedBag()
    {
        return equippedBag;
    }
}
