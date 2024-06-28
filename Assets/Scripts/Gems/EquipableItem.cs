using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableItem : MonoBehaviour
{
    public List<Gem> equippedGems;

    public void EquipGem(Gem gem, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < equippedGems.Count)
        {
            equippedGems[slotIndex] = gem;
        }
    }

    public void UnequipGem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < equippedGems.Count)
        {
            equippedGems[slotIndex] = null;
        }
    }
}

