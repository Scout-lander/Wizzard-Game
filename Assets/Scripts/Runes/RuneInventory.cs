using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneInventory : MonoBehaviour
{
    [Header("Do not edit the Runes. It is updated by scripts.")]
    [Header("You can only update the Capacity.")]
    [Header("")]
    public RuneBagSerializable runeBag = new RuneBagSerializable();

    
    public RuneBagSerializable equippedRuneBag = new RuneBagSerializable();
}
