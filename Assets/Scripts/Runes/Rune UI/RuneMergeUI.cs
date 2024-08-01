using System.Collections;
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

    public GameObject newRunePanelParent; // Parent GameObject for the new rune panel
    public TextMeshProUGUI newRuneNameText; // Text to show the new rune name
    public TextMeshProUGUI newRuneRarityText; // Text to show the new rune rarity

    private List<GameObject> mergeSlots = new List<GameObject>();
    private Rune[] runesInMergeSlots = new Rune[3];
    private int selectedRuneIndex = -1;
    private int selectedMergeSlotIndex = -1;
    private RuneInventory runeInventory;
    private GameObject newRuneSlot; // Slot for displaying the new rune

    public RuneDataNew[] runeDataNewList; // Array to hold references to RuneDataNew ScriptableObjects

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
        UpdateProbabilitiesText();
        addButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(false);
        mergeButton.interactable = false;

        if (newRuneSlot != null)
        {
            Destroy(newRuneSlot);
        }
        newRunePanelParent.SetActive(false); // Ensure the new rune panel is initially inactive
        newRuneNameText.text = "";
        newRuneRarityText.text = "";
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

        if (selectedRuneIndex < 0 || selectedRuneIndex >= runeBag.runes.Count)
        {
            Debug.LogError("Invalid selectedRuneIndex: " + selectedRuneIndex);
            return;
        }

        Rune rune = runeBag.runes[selectedRuneIndex];
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
                            iconImage.sprite = rune.GetIcon();
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
            ShowRuneDetails(runesInMergeSlots[index]);
        }
    }

    private void ShowRuneDetails(Rune rune)
    {
        runeBagUI.runeNameText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(runeBagUI.GetRarityColor(rune.rarity))}>({rune.rarity})</color> {rune.name}";
        runeBagUI.runeDescriptionText.text = runeBagUI.GetRuneDescription(rune);
        runeBagUI.runeNameText.gameObject.SetActive(true);
        runeBagUI.runeDescriptionText.gameObject.SetActive(true);
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
            int runeIndex = runeInventory.runeBag.runes.IndexOf(runesInMergeSlots[index]);
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
                int index = runeInventory.runeBag.runes.IndexOf(rune);
                runeInventory.runeBag.runes.Remove(rune);
                HighlightRuneInInventory(index, false);
            }
            runeInventory.runeBag.runes.Add(newRune);
            ClearMergeSlots();
            runeBagUI.UpdateSlots();
            ShowNewRuneDetails(newRune); // Show the details of the new rune
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
        RuneRarity newRarity = CalculateNewRarity(baseRune.rarity);
        RuneDataNew runeData = FindRuneData(baseRune.name);

        if (runeData == null)
        {
            Debug.LogError("RuneData for " + baseRune.name + " is not found.");
            return null; // Or handle the error appropriately
        }

        Rune newRune = new Rune
        {
            iconName = baseRune.iconName,
            name = baseRune.name,
            description = baseRune.description,
            rarity = newRarity,
            actualStats = InitializeRuneStats(runeData, newRarity),
            takesDamage = baseRune.takesDamage
        };

        if (newRune.actualStats == null)
        {
            Debug.LogError("Failed to initialize rune stats for " + baseRune.name);
            return null; // Or handle the error appropriately
        }

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

    private RuneDataNew FindRuneData(string runeName)
    {
        foreach (var runeData in runeDataNewList)
        {
            if (runeData.name == runeName)
            {
                return runeData;
            }
        }
        return null;
    }

    private RuneDataNew.Stats InitializeRuneStats(RuneDataNew runeDataNew, RuneRarity rarity)
    {
        if (runeDataNew == null)
        {
            Debug.LogError("RuneDataNew is null. Ensure it is properly initialized.");
            return null; // Or return a default value if necessary
        }

        RuneDataNew.Stats minStats;
        RuneDataNew.Stats maxStats;

        // Set the min and max stats based on the rarity
        switch (rarity)
        {
            case RuneRarity.Common:
                minStats = runeDataNew.commonMinimumPossible ?? new RuneDataNew.Stats();
                maxStats = runeDataNew.commonMaximumPossible ?? new RuneDataNew.Stats();
                break;
            case RuneRarity.Uncommon:
                minStats = runeDataNew.uncommonMinimumPossible ?? new RuneDataNew.Stats();
                maxStats = runeDataNew.uncommonMaximumPossible ?? new RuneDataNew.Stats();
                break;
            case RuneRarity.Rare:
                minStats = runeDataNew.rareMinimumPossible ?? new RuneDataNew.Stats();
                maxStats = runeDataNew.rareMaximumPossible ?? new RuneDataNew.Stats();
                break;
            case RuneRarity.Epic:
                minStats = runeDataNew.epicMinimumPossible ?? new RuneDataNew.Stats();
                maxStats = runeDataNew.epicMaximumPossible ?? new RuneDataNew.Stats();
                break;
            case RuneRarity.Legendary:
                minStats = runeDataNew.legendaryMinimumPossible ?? new RuneDataNew.Stats();
                maxStats = runeDataNew.legendaryMaximumPossible ?? new RuneDataNew.Stats();
                break;
            case RuneRarity.Mythic:
                minStats = runeDataNew.mythicMinimumPossible ?? new RuneDataNew.Stats();
                maxStats = runeDataNew.mythicMaximumPossible ?? new RuneDataNew.Stats();
                break;
            default:
                minStats = new RuneDataNew.Stats();
                maxStats = new RuneDataNew.Stats();
                break;
        }

        // Generate stats within the min and max range
        RuneDataNew.Stats rolledStats = new RuneDataNew.Stats
        {
            health = Random.Range(minStats.health, maxStats.health),
            attackSpeed = Random.Range(minStats.attackSpeed, maxStats.attackSpeed),
            luck = Random.Range(minStats.luck, maxStats.luck),
            curse = Random.Range(minStats.curse, maxStats.curse),
            dashCount = Random.Range(minStats.dashCount, maxStats.dashCount),
            dashCooldown = Random.Range(minStats.dashCooldown, maxStats.dashCooldown),
            armor = Random.Range(minStats.armor, maxStats.armor),
            heartRune = Random.Range(minStats.heartRune, maxStats.heartRune),
            lifeRegen = Random.Range(minStats.lifeRegen, maxStats.lifeRegen),
            might = Random.Range(minStats.might, maxStats.might),
            moveSpeed = Random.Range(minStats.moveSpeed, maxStats.moveSpeed)
        };

        return rolledStats;
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

    private void ShowNewRuneDetails(Rune newRune)
    {
        if (newRuneSlot != null)
        {
            Destroy(newRuneSlot);
        }

        newRuneSlot = Instantiate(slotPrefab, newRunePanelParent.transform);
        newRuneSlot.SetActive(true);

        Image slotImage = newRuneSlot.GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.color = runeBagUI.GetRarityColor(newRune.rarity);

            Transform iconTransform = newRuneSlot.transform.Find("Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = newRune.GetIcon();
                    iconImage.color = Color.white; // Ensure the icon is visible
                }
            }

            Transform frameTransform = newRuneSlot.transform.Find("Frame");
            if (frameTransform != null)
            {
                Image frameImage = frameTransform.GetComponent<Image>();
                if (frameImage != null)
                {
                    frameImage.enabled = true; // Ensure the frame is enabled
                }
            }
        }

        newRuneNameText.text = newRune.name;
        newRuneRarityText.text = newRune.rarity.ToString();
        newRuneRarityText.color = runeBagUI.GetRarityColor(newRune.rarity); // Set text color to rarity color

        newRunePanelParent.SetActive(true); // Show the new rune panel

        StartCoroutine(CloseNewRunePanelAfterDelay(5.0f));
    }

    private IEnumerator CloseNewRunePanelAfterDelay(float delay)
    {
        float timeElapsed = 0f;
        while (timeElapsed < delay)
        {
            if (Input.GetMouseButtonDown(0)) // Close panel if mouse is clicked
            {
                newRunePanelParent.SetActive(false);
                yield break;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        newRunePanelParent.SetActive(false);
    }
}
