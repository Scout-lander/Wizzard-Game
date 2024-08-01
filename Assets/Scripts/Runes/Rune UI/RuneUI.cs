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
    public GameObject mergeUI;
    public TMP_Text notificationText;

    private Transform equippedRuneTransform;
    private Transform inventoryRuneTransform;
    public List<Image> runeSlotImages = new List<Image>();
    private List<Image> runeIconImages = new List<Image>();
    private List<Image> frameImages = new List<Image>();
    private List<Image> equippedRuneSlotImages = new List<Image>();
    private List<Image> equippedRuneIconImages = new List<Image>();
    private int selectedRuneIndex = -1;
    private int selectedEquippedRuneIndex = -1;

    public GameObject playerObject;
    private RuneInventory runeInventory;
    private SaveLoadManager saveLoadManager;
    private EquippedRuneStatsUI equippedRuneStatsUI;

    void Start()
    {
        runeInventory = playerObject.GetComponent<RuneInventory>();
        if (runeInventory == null)
        {
            Debug.LogError("RuneInventory component not found on the player object.");
            return;
        }

        saveLoadManager = FindObjectOfType<SaveLoadManager>();
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
        if (index < runeBag.runes.Count && runeBag.runes[index] != null)
        {
            selectedRuneIndex = index;
            selectedEquippedRuneIndex = -1;
            Rune rune = runeBag.runes[index];
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
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    mergeScript.AddRuneToMergeSlot();
                }
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                EquipSelectedRune();
            }
        }
    }

    private void OnEquippedRuneClicked(int index)
    {
        RuneBagSerializable equippedBag = runeInventory.equippedRuneBag;
        if (index < equippedBag.runes.Count && equippedBag.runes[index] != null)
        {
            selectedEquippedRuneIndex = index;
            selectedRuneIndex = -1;
            Rune rune = equippedBag.runes[index];
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

    public string GetRuneDescription(Rune rune)
    {
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
            return $"<color={color}>{statName}</color>: +{statValue:F2}\n";
        }
        else
        {
            string color = isNegativeGood ? "yellow" : "red";
            return $"<color={color}>{statName}</color>: {statValue:F2}\n";
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
        if (selectedRuneIndex >= 0 && selectedRuneIndex < runeBag.runes.Count)
        {
            Rune rune = runeBag.runes[selectedRuneIndex];
            if (equippedBag.runes.Count < equippedBag.maxCapacity)
            {
                int sameNameCount = equippedBag.runes.Count(r => r.name == rune.name);
                if (sameNameCount >= 4)
                {
                    ShowNotification("Can't equip rune. You already have 4 runes with the same name equipped.");
                    return;
                }

                equippedBag.runes.Add(rune);
                runeBag.runes.RemoveAt(selectedRuneIndex);
                UpdateSlots();
                UpdateEquippedSlots();
                ClearSelectedRune();
                SaveRuneBags();
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
        if (selectedEquippedRuneIndex >= 0 && selectedEquippedRuneIndex < equippedBag.runes.Count)
        {
            Rune rune = equippedBag.runes[selectedEquippedRuneIndex];
            if (runeBag.runes.Count < runeBag.maxCapacity)
            {
                runeBag.runes.Add(rune);
                equippedBag.runes.RemoveAt(selectedEquippedRuneIndex);
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
        if (selectedRuneIndex >= 0 && selectedRuneIndex < runeBag.runes.Count)
        {
            runeBag.runes.RemoveAt(selectedRuneIndex);
            UpdateSlots();
            ClearSelectedRune();
            equippedRuneStatsUI.UpdateEquippedRuneStats();
            SaveRuneBags();
        }
        else if (selectedEquippedRuneIndex >= 0 && selectedEquippedRuneIndex < equippedBag.runes.Count)
        {
            equippedBag.runes.RemoveAt(selectedEquippedRuneIndex);
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
            if (i < runeBag.runes.Count && runeBag.runes[i] != null)
            {
                Rune rune = runeBag.runes[i];
                Sprite icon = rune.GetIcon();
                if (icon != null && !IsDestroyed(icon))
                {
                    runeSlotImages[i].color = GetRarityColor(rune.rarity);
                    runeIconImages[i].sprite = icon;
                    runeIconImages[i].color = Color.white;
                }
                else
                {
                    Debug.LogWarning($"Rune '{rune.name}' is missing its icon and will be removed.");
                    runeBag.runes.RemoveAt(i);
                    i--;
                }
            }
            else
            {
                runeSlotImages[i].color = new Color(0, 0, 0, 0.5f);
                runeIconImages[i].sprite = null;
                runeIconImages[i].color = new Color(0, 0, 0, 0.5f);
            }
            frameImages[i].enabled = true;
        }

        SaveRuneBags();
    }

    private bool IsDestroyed(Object obj)
    {
        return obj == null || obj.Equals(null);
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
            if (i < equippedBag.runes.Count && equippedBag.runes[i] != null)
            {
                Rune rune = equippedBag.runes[i];
                Sprite icon = rune.GetIcon();
                if (icon != null)
                {
                    equippedRuneSlotImages[i].color = GetRarityColor(rune.rarity);
                    equippedRuneIconImages[i].sprite = icon;
                    equippedRuneIconImages[i].color = Color.white;
                }
                else
                {
                    equippedRuneSlotImages[i].color = new Color(0, 0, 0, 0.5f);
                    equippedRuneIconImages[i].sprite = null;
                    equippedRuneIconImages[i].color = new Color(0, 0, 0, 0.5f);
                }
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
        runeBag.runes = runeBag.runes
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

        mergeUI.GetComponent<RuneMergeUI>().InitializeMergeUI();
    }

    private void CloseMergeUI()
    {
        mergeUI.SetActive(false);
        openMergeUIButton.gameObject.SetActive(true);
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
        yield return new WaitForSeconds(2f);
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
