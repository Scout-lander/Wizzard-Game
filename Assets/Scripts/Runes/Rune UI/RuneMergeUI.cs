using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuneMergeUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform mergeSlotsParent;
    public Button addButton;
    public Button removeButton;
    public Button mergeButton;
    public RuneUI runeBagUI; // Reference to the RuneUI script
    public TextMeshProUGUI probabilitiesNameText; // Text to show rarity probabilities
    public TextMeshProUGUI probabilitiesProbText; // Text to show rarity probabilities
    public TextMeshProUGUI probabilitiesErrorText; // Text to show error messages

    private List<GameObject> mergeSlots = new List<GameObject>();
    private Rune[] runesInMergeSlots = new Rune[3];
    private int selectedRuneIndex = -1;
    private int selectedMergeSlotIndex = -1;
    private RuneInventory runeInventory;

    void Start()
    {
        Debug.Log("Merger Opened");

        runeInventory = runeBagUI.playerObject.GetComponent<RuneInventory>();
        if (runeInventory == null)
        {
            Debug.LogError("RuneInventory component not found on the player object.");
            return;
        }

        addButton.onClick.AddListener(AddRuneToMergeSlot);
        removeButton.onClick.AddListener(OnRemoveButtonClicked);
        mergeButton.onClick.AddListener(MergeRunes);

        InitializeMergeUI();
    }

    void OnEnable()
    {
        InitializeMergeUI();
    }

    public void InitializeMergeUI()
    {
        ClearMergeSlots();
        //CreateMergeSlots();
        UpdateProbabilitiesText();
        addButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(false);
        mergeButton.interactable = false;
    }

    private void CreateMergeSlots()
    {
        for (int i = 0; i < 3; i++) // Ensure only 3 slots are created
        {
            GameObject slot = Instantiate(slotPrefab, mergeSlotsParent);
            slot.transform.SetParent(mergeSlotsParent, false); // Ensure correct parenting
            slot.SetActive(true); // Ensure the slot is active

            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.color = new Color(0, 0, 0, 0.5f); // Set initial color to semi-transparent
            }
            else
            {
                Debug.LogError("No Image component found on slotPrefab.");
            }

            Button slotButton = slot.GetComponent<Button>() ?? slot.AddComponent<Button>();

            int slotIndex = i; // Capture the current value of i
            slotButton.onClick.AddListener(() => OnMergeSlotClicked(slotIndex));

            mergeSlots.Add(slot);
        }
    }

    public void OnRuneClicked(Rune rune, int index)
    {
        selectedRuneIndex = index;
        addButton.gameObject.SetActive(true);
        removeButton.gameObject.SetActive(false);
    }

    public void AddRuneToMergeSlot()
    {
        if (runeInventory == null)
        {
            Debug.LogError("RuneInventory is not initialized.");
            return;
        }

        var runeBag = runeInventory.runeBag;
        if (runeBag == null)
        {
            Debug.LogError("runeBag is null");
            return;
        }

        if (selectedRuneIndex < 0 || selectedRuneIndex >= runeBag.rune.Count)
        {
            Debug.LogError("Invalid selectedRuneIndex: " + selectedRuneIndex);
            return;
        }

        Rune rune = runeBag.rune[selectedRuneIndex];
        if (rune == null)
        {
            Debug.LogError("Selected rune is null at index: " + selectedRuneIndex);
            return;
        }

        // Check if the rune is already in the merge slots
        if (System.Array.Exists(runesInMergeSlots, r => r == rune))
        {
            Debug.LogError("Rune is already added to the merge slots.");
            return;
        }

        for (int i = 0; i < mergeSlots.Count; i++)
        {
            if (runesInMergeSlots[i] == null)
            {
                runesInMergeSlots[i] = rune;
                Image slotImage = mergeSlots[i].GetComponent<Image>();
                if (slotImage != null)
                {
                    slotImage.color = runeBagUI.GetRarityColor(rune.rarity);

                    Transform iconTransform = mergeSlots[i].transform.Find("Icon");
                    if (iconTransform != null)
                    {
                        Image iconImage = iconTransform.GetComponent<Image>();
                        if (iconImage != null)
                        {
                            iconImage.sprite = rune.icon;
                            iconImage.color = Color.white; // Ensure the icon is visible
                        }
                    }

                    Transform frameTransform = mergeSlots[i].transform.Find("Frame");
                    if (frameTransform != null)
                    {
                        Image frameImage = frameTransform.GetComponent<Image>();
                        if (frameImage != null)
                        {
                            frameImage.enabled = true; // Ensure the frame is enabled
                        }
                    }
                }
                else
                {
                    Debug.LogError("Image component not found on merge slot");
                }

                HighlightRuneInInventory(selectedRuneIndex, true);
                ClearSelectedRune();
                UpdateProbabilitiesText();
                UpdateMergeButtonState();
                break;
            }
        }
    }

    public void OnMergeSlotClicked(int index)
    {
        Debug.Log($"Merge slot {index} clicked.");
        if (runesInMergeSlots[index] != null)
        {
            selectedMergeSlotIndex = index;
            addButton.gameObject.SetActive(false);
            removeButton.gameObject.SetActive(true);
        }
    }

    private void OnRemoveButtonClicked()
    {
        Debug.Log("OnRemoveButtonClicked method called");
        if (selectedMergeSlotIndex >= 0 && selectedMergeSlotIndex < mergeSlots.Count)
        {
            RemoveRuneFromMergeSlot(selectedMergeSlotIndex);
        }
    }

    public void RemoveRuneFromMergeSlot(int index)
    {
        if (runesInMergeSlots[index] != null)
        {
            int runeIndex = runeInventory.runeBag.rune.IndexOf(runesInMergeSlots[index]);
            HighlightRuneInInventory(runeIndex, false);
            Image slotImage = mergeSlots[index].GetComponent<Image>();
            slotImage.color = new Color(0, 0, 0, 0.5f); // Set color to semi-transparent
            slotImage.material = null; // Clear the material

            Transform iconTransform = mergeSlots[index].transform.Find("Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = null;
                    iconImage.color = new Color(0, 0, 0, 0.5f); // Set color to semi-transparent
                }
            }

            Transform frameTransform = mergeSlots[index].transform.Find("Frame");
            if (frameTransform != null)
            {
                Image frameImage = frameTransform.GetComponent<Image>();
                if (frameImage != null)
                {
                    frameImage.enabled = true; // Ensure the frame is enabled
                }
            }

            runesInMergeSlots[index] = null;
            Debug.Log($"Rune removed from merge slot {index}");
            removeButton.gameObject.SetActive(false); // Hide remove button after removal
            UpdateProbabilitiesText();
            UpdateMergeButtonState();
        }
    }

    private void HighlightRuneInInventory(int index, bool highlight)
    {
        Color color = highlight ? Color.grey : Color.white;
        runeBagUI.runeSlotImages[index].color = color;
    }

    private void ClearSelectedRune()
    {
        selectedRuneIndex = -1;
        addButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(false);
    }

    public void MergeRunes()
    {
        Debug.Log("MergeRunes method called");
        if (CanMergeRunes())
        {
            Rune newRune = CreateNewRune(runesInMergeSlots[0]);
            foreach (Rune rune in runesInMergeSlots)
            {
                int index = runeInventory.runeBag.rune.IndexOf(rune);
                runeInventory.runeBag.rune.Remove(rune);
                HighlightRuneInInventory(index, false);
            }
            runeInventory.runeBag.rune.Add(newRune);
            ClearMergeSlots();
            //CreateMergeSlots(); // Re-create merge slots after clearing
            runeBagUI.UpdateSlots();
            Debug.Log("Runes merged successfully");
        }
        else
        {
            Debug.Log("Cannot merge runes: slots are not filled or runes are not the same type.");
        }
    }

    private bool CanMergeRunes()
    {
        if (runesInMergeSlots[0] == null || runesInMergeSlots[1] == null || runesInMergeSlots[2] == null)
            return false;

        return runesInMergeSlots[0].name == runesInMergeSlots[1].name && runesInMergeSlots[1].name == runesInMergeSlots[2].name;
    }

    private Rune CreateNewRune(Rune baseRune)
    {
        Rune newRune = new Rune
        {
            icon = baseRune.icon,
            name = baseRune.name,
            description = baseRune.description,
            rarity = CalculateNewRarity(baseRune.rarity),
            actualStats = baseRune.actualStats, // You might want to randomize or average stats here
            takesDamage = baseRune.takesDamage
        };
        return newRune;
    }

    private RuneRarity CalculateNewRarity(RuneRarity baseRarity)
    {
        // Implement logic to determine the new rarity based on merging probabilities
        // For simplicity, you can return the next rarity level or any other logic
        switch (baseRarity)
        {
            case RuneRarity.Common:
                return RuneRarity.Uncommon;
            case RuneRarity.Uncommon:
                return RuneRarity.Rare;
            case RuneRarity.Rare:
                return RuneRarity.Epic;
            case RuneRarity.Epic:
                return RuneRarity.Legendary;
            case RuneRarity.Legendary:
                return RuneRarity.Mythic;
            default:
                return RuneRarity.Mythic;
        }
    }

    private void ClearMergeSlots()
    {
        foreach (var slot in mergeSlots)
        {
            Destroy(slot);
        }
        mergeSlots.Clear();
        runesInMergeSlots = new Rune[3];
        CreateMergeSlots();
        UpdateMergeButtonState();
        UpdateProbabilitiesText();
    }

    private void UpdateMergeButtonState()
    {
        mergeButton.interactable = CanMergeRunes();
    }

    private void UpdateProbabilitiesText()
    {
        if (runesInMergeSlots[0] == null || runesInMergeSlots[1] == null || runesInMergeSlots[2] == null)
        {
            probabilitiesErrorText.text = "<align=center>Waiting for runes</align>";
            probabilitiesNameText.text = "";
            probabilitiesProbText.text = "";
            return;
        }

        if (!CanMergeRunes())
        {
            probabilitiesErrorText.text = "<align=center>Needs to be 3 of the same kind of rune</align>";
            probabilitiesNameText.text = "";
            probabilitiesProbText.text = "";
            return;
        }

        float[] probabilities = RuneMergeRarityCalculator.CalculateRarityProbabilities(runesInMergeSlots);
        probabilitiesErrorText.text = "";

        probabilitiesNameText.text = $"Probabilities:\n\n" +
            "<color=grey>Common</color>:\n" +
            "<color=green>Uncommon</color>:\n" +
            "<color=blue>Rare</color>:\n" +
            "<color=purple>Epic</color>:\n" +
            "<color=yellow>Legendary</color>:\n" + 
            "<color=red>Mythic</color>:\n";

        probabilitiesProbText.text = $"\n\n" +
            $"{probabilities[0]:F2}%\n" +
            $"{probabilities[1]:F2}%\n" +
            $"{probabilities[2]:F2}%\n" +
            $"{probabilities[3]:F2}%\n" +
            $"{probabilities[4]:F2}%\n" +
            $"{probabilities[5]:F2}%\n";
    }
}
