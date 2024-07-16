using System.Collections.Generic;
using UnityEngine;

public class RuneInventoryManager : MonoBehaviour
{
    public static RuneInventoryManager Instance;

    public RuneBagSerializable runeBag = new RuneBagSerializable();
    public RuneBagSerializable equippedBag = new RuneBagSerializable();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionally persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public RuneBagSerializable GetRuneBag()
    {
        return runeBag;
    }

    public RuneBagSerializable GetEquippedBag()
    {
        return equippedBag;
    }
}
