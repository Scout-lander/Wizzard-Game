using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebuffUI : MonoBehaviour
{
    public Transform debuffContainer; // Reference to the parent container for debuff slots
    public Sprite iceSlowedIcon;      // Icon for Ice Slowed debuff
    public Sprite stunnedIcon;        // Icon for Stunned debuff
    public Sprite burningIcon;        // Icon for Burning debuff
    public Sprite poisonedIcon;       // Icon for Poisoned debuff

    private List<Image> debuffImages = new List<Image>(); // List to keep track of debuff images

    // Player references
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;

    void Start()
    {
        if (debuffContainer == null)
        {
            Debug.LogError("DebuffUI: No debuffContainer assigned.");
            return;
        }

        playerMovement = FindObjectOfType<PlayerMovement>();
        playerStats = FindObjectOfType<PlayerStats>();

        if (playerMovement == null || playerStats == null)
        {
            Debug.LogError("DebuffUI: PlayerMovement or PlayerStats script not found.");
            return;
        }

        // Get all debuff images and deactivate their parent slots
        for (int i = 0; i < debuffContainer.childCount; i++)
        {
            Transform slot = debuffContainer.GetChild(i);
            Image debuffImage = slot.GetComponent<Image>();
            if (debuffImage != null)
            {
                debuffImages.Add(debuffImage);
                slot.gameObject.SetActive(false); // Deactivate the parent slot
            }
        }

        // Start the coroutine to update the UI every second
        StartCoroutine(UpdateUIRoutine());
    }

    // Coroutine to update the UI every second
    private IEnumerator UpdateUIRoutine()
    {
        while (true)
        {
            // Track the current slot index
            int slotIndex = 0;

            // Update Ice Slowed debuff
            if (playerMovement.IsIceSlowed && slotIndex < debuffImages.Count)
            {
                UpdateDebuffSlot(slotIndex, iceSlowedIcon, "IceSlowed");
                slotIndex++;
            }

            // Update Stunned debuff
            if (playerMovement.isStunned && slotIndex < debuffImages.Count)
            {
                UpdateDebuffSlot(slotIndex, stunnedIcon, "Stunned");
                slotIndex++;
            }

            // Update Burning debuff
            if (playerStats.isBurning && slotIndex < debuffImages.Count)
            {
                UpdateDebuffSlot(slotIndex, burningIcon, "Burning");
                slotIndex++;
            }

            // Update Poisoned debuff
            if (playerStats.isPoisoned && slotIndex < debuffImages.Count)
            {
                UpdateDebuffSlot(slotIndex, poisonedIcon, "Poisoned");
                slotIndex++;
            }

            // Deactivate remaining slots if any
            for (int i = slotIndex; i < debuffImages.Count; i++)
            {
                RemoveDebuffSlot(i);
            }

            yield return new WaitForSeconds(1.0f); // Wait for one second
        }
    }

    // Update a specific debuff slot with the correct icon
    private void UpdateDebuffSlot(int slotIndex, Sprite debuffIcon, string debuffName)
    {
        if (slotIndex >= debuffImages.Count)
        {
            Debug.LogWarning("DebuffUI: Slot index out of range.");
            return;
        }

        Image debuffImage = debuffImages[slotIndex];

        // Activate the debuff icon
        debuffImage.gameObject.SetActive(true);

        // Set the debuff icon
        debuffImage.sprite = debuffIcon;

        // Ensure the image is fully filled
        debuffImage.fillAmount = 1.0f;

        //Debug.Log($"DebuffUI: {debuffName} debuff applied to slot {slotIndex}.");
    }

    // Remove a specific debuff slot
    private void RemoveDebuffSlot(int slotIndex)
    {
        if (slotIndex >= debuffImages.Count)
        {
            Debug.LogWarning("DebuffUI: Slot index out of range.");
            return;
        }

        Image debuffImage = debuffImages[slotIndex];

        // Deactivate the debuff icon
        debuffImage.gameObject.SetActive(false);

       //Debug.Log($"DebuffUI: Debuff slot {slotIndex} removed.");
    }
}
