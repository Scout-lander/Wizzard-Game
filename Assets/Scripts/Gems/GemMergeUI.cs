using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GemMergeUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform mergeSlotsParent;
    public Button addButton;
    public Button removeButton;
    public Button mergeButton;
    public GemBagUI gemBagUI; // Reference to the GemBagUI script
    public TextMeshProUGUI probabilitiesText; // Text to show rarity probabilities

    private List<GameObject> mergeSlots = new List<GameObject>();
    private GemData[] gemsInMergeSlots = new GemData[3];
    private int selectedGemIndex = -1;
    private int selectedMergeSlotIndex = -1;

    void Start()
    {
        Debug.Log("Start method called");
        addButton.onClick.AddListener(AddGemToMergeSlot);
        removeButton.onClick.AddListener(OnRemoveButtonClicked);
        mergeButton.onClick.AddListener(MergeGems);

        addButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(false);
        mergeButton.interactable = false;
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable method called");
        ClearMergeSlots();
        CreateMergeSlots();
        UpdateProbabilitiesText();
    }

    private void CreateMergeSlots()
    {
        Debug.Log("CreateMergeSlots method called");
        for (int i = 0; i < 3; i++)
        {
            GameObject slot = Instantiate(slotPrefab, mergeSlotsParent);
            Image slotImage = slot.GetComponent<Image>();
            slotImage.color = new Color(0, 0, 0, 0.5f); // Set initial color to semi-transparent

            Button slotButton = slot.GetComponent<Button>() ?? slot.AddComponent<Button>();

            int slotIndex = i; // Capture the current value of i
            slotButton.onClick.AddListener(() => OnMergeSlotClicked(slotIndex));

            mergeSlots.Add(slot);
        }
    }

    public void OnGemClicked(GemData gem, int index)
    {
        selectedGemIndex = index;
        Debug.Log($"Gem clicked: {gem.gemName}, Index: {index}");
        addButton.gameObject.SetActive(true);
        removeButton.gameObject.SetActive(false);
    }

    public void AddGemToMergeSlot()
    {
        Debug.Log($"AddGemToMergeSlot called. Selected Gem Index: {selectedGemIndex}");

        if (selectedGemIndex >= 0 && selectedGemIndex < GemInventoryManager.Instance.GetGemBag().gems.Count)
        {
            GemData gem = GemInventoryManager.Instance.GetGemBag().gems[selectedGemIndex];
            Debug.Log($"Adding Gem: {gem.gemName} to merge slot.");
            for (int i = 0; i < mergeSlots.Count; i++)
            {
                if (gemsInMergeSlots[i] == null)
                {
                    gemsInMergeSlots[i] = gem;
                    Image slotImage = mergeSlots[i].GetComponent<Image>();
                    slotImage.sprite = gem.icon;
                    slotImage.color = Color.white; // Set color to white
                    HighlightGemInInventory(selectedGemIndex, true);
                    ClearSelectedGem();
                    UpdateProbabilitiesText();
                    UpdateMergeButtonState();
                    Debug.Log($"Gem added to merge slot {i}");
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Invalid selected gem index or gem not found in inventory.");
        }
    }

    public void OnMergeSlotClicked(int index)
    {
        Debug.Log($"Merge slot {index} clicked.");
        if (gemsInMergeSlots[index] != null)
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
            RemoveGemFromMergeSlot(selectedMergeSlotIndex);
        }
    }

    public void RemoveGemFromMergeSlot(int index)
    {
        Debug.Log($"RemoveGemFromMergeSlot called for index {index}");
        if (gemsInMergeSlots[index] != null)
        {
            int gemIndex = GemInventoryManager.Instance.GetGemBag().gems.IndexOf(gemsInMergeSlots[index]);
            HighlightGemInInventory(gemIndex, false);
            Image slotImage = mergeSlots[index].GetComponent<Image>();
            slotImage.sprite = null;
            slotImage.color = new Color(0, 0, 0, 0.5f); // Set color to semi-transparent
            gemsInMergeSlots[index] = null;
            Debug.Log($"Gem removed from merge slot {index}");
            removeButton.gameObject.SetActive(false); // Hide remove button after removal
            UpdateProbabilitiesText();
            UpdateMergeButtonState();
        }
    }

    private void HighlightGemInInventory(int index, bool highlight)
    {
        Debug.Log($"HighlightGemInInventory called for index {index} with highlight {highlight}");
        Color color = highlight ? Color.grey : Color.white;
        gemBagUI.gemImages[index].color = color;
    }

    private void ClearSelectedGem()
    {
        Debug.Log("ClearSelectedGem method called");
        selectedGemIndex = -1;
        addButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(false);
    }

    public void MergeGems()
    {
        Debug.Log("MergeGems method called");
        if (CanMergeGems())
        {
            GemData newGem = CreateNewGem(gemsInMergeSlots[0]);
            foreach (GemData gem in gemsInMergeSlots)
            {
                int index = GemInventoryManager.Instance.GetGemBag().gems.IndexOf(gem);
                GemInventoryManager.Instance.GetGemBag().RemoveGem(gem);
                HighlightGemInInventory(index, false);
            }
            GemInventoryManager.Instance.GetGemBag().AddGem(newGem);
            ClearMergeSlots();
            CreateMergeSlots(); // Re-create merge slots after clearing
            gemBagUI.UpdateSlots();
            Debug.Log("Gems merged successfully");
        }
        else
        {
            Debug.Log("Cannot merge gems: slots are not filled or gems are not the same type.");
        }
    }

    private bool CanMergeGems()
    {
        Debug.Log("CanMergeGems method called");
        if (gemsInMergeSlots[0] == null || gemsInMergeSlots[1] == null || gemsInMergeSlots[2] == null)
            return false;

        return gemsInMergeSlots[0].gemName == gemsInMergeSlots[1].gemName && gemsInMergeSlots[1].gemName == gemsInMergeSlots[2].gemName;
    }

    private GemData CreateNewGem(GemData baseGem)
    {
        Debug.Log("CreateNewGem method called");
        GemData newGem = baseGem.Clone();
        newGem.InitializeRandomValues();
        return newGem;
    }

    private void ClearMergeSlots()
    {
        Debug.Log("ClearMergeSlots method called");
        foreach (var slot in mergeSlots)
        {
            Destroy(slot);
        }
        mergeSlots.Clear();
        gemsInMergeSlots = new GemData[3];
        UpdateMergeButtonState();
        UpdateProbabilitiesText();
    }

    private void UpdateMergeButtonState()
    {
        mergeButton.interactable = CanMergeGems();
    }

    private void UpdateProbabilitiesText()
    {
        Debug.Log("UpdateProbabilitiesText method called");

        if (gemsInMergeSlots[0] == null || gemsInMergeSlots[1] == null || gemsInMergeSlots[2] == null)
        {
            probabilitiesText.text = "Waiting for gems";
            return;
        }

        if (!CanMergeGems())
        {
            probabilitiesText.text = "Needs to be 3 of the same kind of gem";
            return;
        }

        float[] probabilities = GemMergeRarityCalculator.CalculateRarityProbabilities(gemsInMergeSlots);
        
        probabilitiesText.text = $"Probabilities:\n" +
            $"<color=grey>Common</color>: {probabilities[0]:F2}%\n" +
            $"<color=green>Uncommon</color>: {probabilities[1]:F2}%\n" +
            $"<color=blue>Rare</color>: {probabilities[2]:F2}%\n" +
            $"<color=purple>Epic</color>: {probabilities[3]:F2}%\n" +
            $"<color=yellow>Legendary</color>: {probabilities[4]:F2}%\n" + 
            $"<color=red>Mithic</color>: {probabilities[5]:F2}%";
    }
}
