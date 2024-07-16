using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuneUI : MonoBehaviour
{
    public GameObject runeSlotPrefab;
    public TMP_Text runeNameText;
    public TMP_Text runeDescriptionText;
    public Button equipButton;
    public Button unequipButton;
    public Button destroyButton;
    public Button sortButton;
    public Button openMergeUIButton;
    public Button closeMergeUIButton;
    public GameObject mergeUI; // The merge UI panel
    public TMP_Text notificationText;  // Reference to the TMP text component

    private Transform equippedRuneTransform;
    private Transform inventoryRuneTransform;
    public List<Image> runeSlotImages = new List<Image>(); // Changed to public
    private List<Image> runeIconImages = new List<Image>();
    private List<Image> frameImages = new List<Image>();
    private List<Image> equippedRuneSlotImages = new List<Image>();
    private List<Image> equippedRuneIconImages = new List<Image>();
    private int selectedRuneIndex = -1;
    private int selectedEquippedRuneIndex = -1;

    public GameObject playerObject;  // Public field for the player object
    private RuneInventory runeInventory;
    private SaveLoadManager saveLoadManager; // Reference to the SaveLoadManager
    private EquippedRuneStatsUI equippedRuneStatsUI; // Reference to the EquippedRuneStatsUI

    void Start()
    {
        // Get the RuneInventory component from the player object
        runeInventory = playerObject.GetComponent<RuneInventory>();

        if (runeInventory == null)
        {
            Debug.LogError("RuneInventory component not found on the player object.");
            return;
        }

        // Get the SaveLoadManager component
        saveLoadManager = FindObjectOfType<SaveLoadManager>(); // Get the SaveLoadManager component
        equippedRuneStatsUI = FindObjectOfType<EquippedRuneStatsUI>();

        equippedRuneTransform = transform.Find("Rune Equipped");
        inventoryRuneTransform = transform.Find("Rune Inventory");

        mergeUI.SetActive(false);

        CreateSlots();
        CreateEquippedSlots();
        UpdateSlots();
        UpdateEquippedSlots();

        equipButton.onClick.AddListener(EquipSelectedRune);
        unequipButton.onClick.AddListener(UnequipSelectedRune);
        destroyButton.onClick.AddListener(DestroySelectedRune);
        sortButton.onClick.AddListener(SortRunesByTypeAndRarity);
        openMergeUIButton.onClick.AddListener(OpenMergeUI);
        closeMergeUIButton.onClick.AddListener(CloseMergeUI);

        ClearSelectedRune();
    }

    void OnEnable()
    {
        if (runeInventory != null)
        {
            UpdateSlots();
            UpdateEquippedSlots();
            equippedRuneStatsUI.UpdateEquippedRuneStats();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseRuneBagUI();
        }
    }

    private void CreateSlots()
    {
        foreach (Transform child in inventoryRuneTransform)
        {
            Destroy(child.gameObject);
        }
        runeSlotImages.Clear();
        runeIconImages.Clear();
        frameImages.Clear();

        for (int i = 0; i < runeInventory.runeBag.maxCapacity; i++)
        {
            GameObject slotObj = Instantiate(runeSlotPrefab, inventoryRuneTransform);
            Image runeSlotImage = slotObj.GetComponent<Image>();
            Image runeIconImage = slotObj.transform.Find("Icon").GetComponent<Image>();
            Image frameImage = slotObj.transform.Find("Frame").GetComponent<Image>();

            runeSlotImages.Add(runeSlotImage);
            runeIconImages.Add(runeIconImage);
            frameImages.Add(frameImage);

            int index = i;
            Button slotButton = runeSlotImage.gameObject.AddComponent<Button>();
            slotButton.onClick.AddListener(() => OnRuneClicked(index));
        }
    }

    private void CreateEquippedSlots()
    {
        foreach (Transform child in equippedRuneTransform)
        {
            Destroy(child.gameObject);
        }
        equippedRuneSlotImages.Clear();
        equippedRuneIconImages.Clear();

        for (int i = 0; i < runeInventory.equippedRuneBag.maxCapacity; i++)
        {
            GameObject slotObj = Instantiate(runeSlotPrefab, equippedRuneTransform);
            Image runeSlotImage = slotObj.GetComponent<Image>();
            Image runeIconImage = slotObj.transform.Find("Icon").GetComponent<Image>();

            equippedRuneSlotImages.Add(runeSlotImage);
            equippedRuneIconImages.Add(runeIconImage);

            int index = i;
            Button slotButton = runeSlotImage.gameObject.AddComponent<Button>();
            slotButton.onClick.AddListener(() => OnEquippedRuneClicked(index));
        }
    }

    private void OnRuneClicked(int index)
    {
        RuneBagSerializable runeBag = runeInventory.runeBag;
        if (index < runeBag.rune.Count && runeBag.rune[index] != null)
        {
            selectedRuneIndex = index;
            selectedEquippedRuneIndex = -1;
            Rune rune = runeBag.rune[index];
            runeDescriptionText.text = GetRuneDescription(rune);
            runeNameText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(GetRarityColor(rune.rarity))}>({rune.rarity})</color> {rune.name}";

            runeNameText.gameObject.SetActive(true);
            runeDescriptionText.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(true);
            unequipButton.gameObject.SetActive(false);
            destroyButton.gameObject.SetActive(true);

            if (mergeUI.activeSelf)
            {
                RuneMergeUI mergeScript = mergeUI.GetComponent<RuneMergeUI>();
                mergeScript.OnRuneClicked(rune, index);
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                EquipSelectedRune();
            }
        }
    }

    private void OnEquippedRuneClicked(int index)
    {
        RuneBagSerializable equippedBag = runeInventory.equippedRuneBag;
        if (index < equippedBag.rune.Count && equippedBag.rune[index] != null)
        {
            selectedEquippedRuneIndex = index;
            selectedRuneIndex = -1;
            Rune rune = equippedBag.rune[index];
            runeDescriptionText.text = GetRuneDescription(rune);
            runeNameText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(GetRarityColor(rune.rarity))}>({rune.rarity})</color> {rune.name}";

            runeNameText.gameObject.SetActive(true);
            runeDescriptionText.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
            unequipButton.gameObject.SetActive(true);
            destroyButton.gameObject.SetActive(true);

            if (mergeUI.activeSelf)
            {
                RuneMergeUI mergeScript = mergeUI.GetComponent<RuneMergeUI>();
                mergeScript.OnRuneClicked(rune, index);
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                UnequipSelectedRune();
            }
        }
    }

    private string GetRuneDescription(Rune rune)
    {
        // Customize the description as per your game's requirements
        string description = rune.description;

        if (rune.actualStats != null)
        {
            description += $"\n\n{FormatStat("Max Health", rune.actualStats.health)}" +
                           $"{FormatStat("Attack Speed", rune.actualStats.attackSpeed)}" +
                           $"{FormatStat("Luck", rune.actualStats.luck)}" +
                           $"{FormatStat("Curse", rune.actualStats.curse, isNegativeGood: true)}" +
                           $"{FormatStat("Dash Count", rune.actualStats.dashCount)}" +
                           $"{FormatStat("Dash Cooldown", rune.actualStats.dashCooldown, isNegativeGood: true)}" +
                           $"{FormatStat("Armor", rune.actualStats.armor)}" +
                           $"{FormatStat("Gem Health", rune.actualStats.heartRune)}" +
                           $"{FormatStat("Life Regen", rune.actualStats.lifeRegen)}" +
                           $"{FormatStat("Might", rune.actualStats.might)}" +
                           $"{FormatStat("Move Speed", rune.actualStats.moveSpeed)}";
        }

        return description;
    }

    private string FormatStat(string statName, float statValue, bool isNegativeGood = false)
    {
        if (statValue == 0)
        {
            return string.Empty;
        }
        else if (statValue > 0)
        {
            string color = isNegativeGood ? "red" : "yellow";
            return $"<color={color}>{statName}</color>: +{statValue}\n";
        }
        else
        {
            string color = isNegativeGood ? "yellow" : "red";
            return $"<color={color}>{statName}</color>: {statValue}\n";
        }
    }

    private string FormatStat(string statName, int statValue, bool isNegativeGood = false)
    {
        if (statValue == 0)
        {
            return string.Empty;
        }
        else if (statValue > 0)
        {
            string color = isNegativeGood ? "red" : "yellow";
            return $"<color={color}>{statName}</color>: +{statValue}\n";
        }
        else
        {
            string color = isNegativeGood ? "yellow" : "red";
            return $"<color={color}>{statName}</color>: {statValue}\n";
        }
    }

    private void EquipSelectedRune()
    {
        RuneBagSerializable runeBag = runeInventory.runeBag;
        RuneBagSerializable equippedBag = runeInventory.equippedRuneBag;
        if (selectedRuneIndex >= 0 && selectedRuneIndex < runeBag.rune.Count)
        {
            Rune rune = runeBag.rune[selectedRuneIndex];
            if (equippedBag.rune.Count < equippedBag.maxCapacity)
            {
                equippedBag.rune.Add(rune);
                runeBag.rune.RemoveAt(selectedRuneIndex);
                UpdateSlots();
                UpdateEquippedSlots();
                ClearSelectedRune();
                SaveRuneBags();// Save the rune bags
                equippedRuneStatsUI.UpdateEquippedRuneStats();
            }
            else
            {
                ShowNotification("Can't equip rune. Equipped bag is full.");
            }
        }
        else
        {
            Debug.Log("Invalid rune index selected for equipping.");
        }
    }

    private void UnequipSelectedRune()
    {
        RuneBagSerializable runeBag = runeInventory.runeBag;
        RuneBagSerializable equippedBag = runeInventory.equippedRuneBag;
        if (selectedEquippedRuneIndex >= 0 && selectedEquippedRuneIndex < equippedBag.rune.Count)
        {
            Rune rune = equippedBag.rune[selectedEquippedRuneIndex];
            if (runeBag.rune.Count < runeBag.maxCapacity)
            {
                runeBag.rune.Add(rune);
                equippedBag.rune.RemoveAt(selectedEquippedRuneIndex);
                UpdateSlots();
                UpdateEquippedSlots();
                ClearSelectedRune();
                equippedRuneStatsUI.UpdateEquippedRuneStats();
                SaveRuneBags();
            }
            else
            {
                ShowNotification("Can't unequip rune. Inventory bag is full.");
            }
        }
        else
        {
            Debug.Log("Invalid rune index selected for unequipping.");
        }
    }

    private void DestroySelectedRune()
    {
        RuneBagSerializable runeBag = runeInventory.runeBag;
        RuneBagSerializable equippedBag = runeInventory.equippedRuneBag;
        if (selectedRuneIndex >= 0 && selectedRuneIndex < runeBag.rune.Count)
        {
            runeBag.rune.RemoveAt(selectedRuneIndex);
            UpdateSlots();
            ClearSelectedRune();
            equippedRuneStatsUI.UpdateEquippedRuneStats();
            SaveRuneBags();
        }
        else if (selectedEquippedRuneIndex >= 0 && selectedEquippedRuneIndex < equippedBag.rune.Count)
        {
            equippedBag.rune.RemoveAt(selectedEquippedRuneIndex);
            UpdateEquippedSlots();
            ClearSelectedRune();
            SaveRuneBags();
        }
    }

    public void UpdateSlots()
    {
        if (runeInventory == null || runeInventory.runeBag == null)
        {
            Debug.LogError("RuneInventory or runeBag is not initialized.");
            return;
        }

        RuneBagSerializable runeBag = runeInventory.runeBag;
        for (int i = 0; i < runeSlotImages.Count; i++)
        {
            if (i < runeBag.rune.Count && runeBag.rune[i] != null)
            {
                Rune rune = runeBag.rune[i];
                runeSlotImages[i].color = GetRarityColor(rune.rarity);
                runeIconImages[i].sprite = rune.icon;
                runeIconImages[i].color = Color.white;
            }
            else
            {
                runeSlotImages[i].color = new Color(0, 0, 0, 0.5f);
                runeIconImages[i].sprite = null;
                runeIconImages[i].color = new Color(0, 0, 0, 0.5f);
            }
            frameImages[i].enabled = true;
        }
    }

    public void UpdateEquippedSlots()
    {
        if (runeInventory == null || runeInventory.equippedRuneBag == null)
        {
            Debug.LogError("RuneInventory or equippedRuneBag is not initialized.");
            return;
        }

        RuneBagSerializable equippedBag = runeInventory.equippedRuneBag;
        for (int i = 0; i < equippedRuneSlotImages.Count; i++)
        {
            if (i < equippedBag.rune.Count && equippedBag.rune[i] != null)
            {
                Rune rune = equippedBag.rune[i];
                equippedRuneSlotImages[i].color = GetRarityColor(rune.rarity);
                equippedRuneIconImages[i].sprite = rune.icon;
                equippedRuneIconImages[i].color = Color.white;
            }
            else
            {
                equippedRuneSlotImages[i].color = new Color(0, 0, 0, 0.5f);
                equippedRuneIconImages[i].sprite = null;
                equippedRuneIconImages[i].color = new Color(0, 0, 0, 0.5f);
            }
        }
    }

    private void ClearSelectedRune()
    {
        selectedRuneIndex = -1;
        selectedEquippedRuneIndex = -1;
        runeNameText.text = "";
        runeDescriptionText.text = "";
        runeNameText.gameObject.SetActive(false);
        runeDescriptionText.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        destroyButton.gameObject.SetActive(false);
    }

    public Color GetRarityColor(RuneRarity rarity)
    {
        switch (rarity)
        {
            case RuneRarity.Common:
                return Color.gray;
            case RuneRarity.Uncommon:
                return Color.green;
            case RuneRarity.Rare:
                return Color.blue;
            case RuneRarity.Epic:
                return Color.magenta;
            case RuneRarity.Legendary:
                return Color.yellow;
            case RuneRarity.Mythic:
                return Color.red;
            default:
                return Color.white;
        }
    }

    private void SortRunesByTypeAndRarity()
    {
        RuneBagSerializable runeBag = runeInventory.runeBag;
        runeBag.rune = runeBag.rune
            .OrderBy(rune => rune.name)
            .ThenBy(rune => rune.rarity)
            .ToList();
        UpdateSlots();
    }

    private void OpenMergeUI()
    {
        mergeUI.SetActive(true);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        destroyButton.gameObject.SetActive(false);
        openMergeUIButton.gameObject.SetActive(false);

        // Initialize the merge UI when opened
        mergeUI.GetComponent<RuneMergeUI>().InitializeMergeUI();
    }

    private void CloseMergeUI()
    {
        mergeUI.SetActive(false);
        openMergeUIButton.gameObject.SetActive(true);
        // Re-enable equip and destroy buttons only if a rune is selected
        if (selectedRuneIndex >= 0)
        {
            equipButton.gameObject.SetActive(true);
            destroyButton.gameObject.SetActive(true);
        }
        else if (selectedEquippedRuneIndex >= 0)
        {
            unequipButton.gameObject.SetActive(true);
            destroyButton.gameObject.SetActive(true);
        }
    }

    private void CloseRuneBagUI()
    {
        gameObject.SetActive(false);
    }

    private void ShowNotification(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(2f); // Show the message for 2 seconds
        notificationText.gameObject.SetActive(false);
    }

    private void SaveRuneBags()
    {
        if (saveLoadManager != null && runeInventory != null)
        {
            saveLoadManager.SaveRuneBags(runeInventory.runeBag, runeInventory.equippedRuneBag);
        }
    }
}
