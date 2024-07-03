using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GemBagUI : MonoBehaviour
{
    public GameObject gemSlotPrefab;
    public TMP_Text gemNameText;
    public TMP_Text gemDescriptionText;
    public Button equipButton;
    public Button unequipButton;
    public Button destroyButton;
    public Button sortButton;
    public Button openMergeUIButton;
    public Button closeMergeUIButton;
    public EquippedGemStatsUI equippedGemStatsUI;
    public GameObject mergeUI; // The merge UI panel
    public TMP_Text notificationText;  // Reference to the TMP text component

    private Transform equippedGemTransform;
    private Transform inventoryGemTransform;
    public List<Image> gemImages = new List<Image>(); // Changed to public
    private List<Image> frameImages = new List<Image>();
    private List<Image> equippedGemImages = new List<Image>();
    private int selectedGemIndex = -1;
    private int selectedEquippedGemIndex = -1;

    void Start()
    {
        equippedGemTransform = transform.Find("Gem Equipped");
        inventoryGemTransform = transform.Find("Gem Inventory");

        mergeUI.SetActive(false);

        CreateSlots();
        CreateEquippedSlots();
        UpdateSlots();
        UpdateEquippedSlots();

        equipButton.onClick.AddListener(EquipSelectedGem);
        unequipButton.onClick.AddListener(UnequipSelectedGem);
        destroyButton.onClick.AddListener(DestroySelectedGem);
        sortButton.onClick.AddListener(SortGemsByName);
        openMergeUIButton.onClick.AddListener(OpenMergeUI);
        closeMergeUIButton.onClick.AddListener(CloseMergeUI);

        ClearSelectedGem();
    }

    void OnEnable()
    {
        UpdateSlots();
        UpdateEquippedSlots();
    }

    private void CreateSlots()
    {
        foreach (Transform child in inventoryGemTransform)
        {
            Destroy(child.gameObject);
        }
        gemImages.Clear();
        frameImages.Clear();

        for (int i = 0; i < GemInventoryManager.Instance.GetGemBag().maxCapacity; i++)
        {
            GameObject slotObj = Instantiate(gemSlotPrefab, inventoryGemTransform);
            Image gemImage = slotObj.GetComponent<Image>();
            Transform frameTransform = slotObj.transform.Find("Frame");
            Image frameImage = frameTransform.GetComponent<Image>();

            gemImages.Add(gemImage);
            frameImages.Add(frameImage);

            int index = i;
            gemImage.gameObject.AddComponent<Button>().onClick.AddListener(() => OnGemClicked(index));
        }
    }

    private void CreateEquippedSlots()
    {
        foreach (Transform child in equippedGemTransform)
        {
            Destroy(child.gameObject);
        }
        equippedGemImages.Clear();

        for (int i = 0; i < GemInventoryManager.Instance.GetEquippedBag().maxCapacity; i++)
        {
            GameObject slotObj = Instantiate(gemSlotPrefab, equippedGemTransform);
            Image gemImage = slotObj.GetComponent<Image>();
            equippedGemImages.Add(gemImage);

            int index = i;
            gemImage.gameObject.AddComponent<Button>().onClick.AddListener(() => OnEquippedGemClicked(index));
        }
    }

    private void OnGemClicked(int index)
    {
        GemBag gemBag = GemInventoryManager.Instance.GetGemBag();
        if (index < gemBag.gems.Count && gemBag.gems[index] != null)
        {
            selectedGemIndex = index;
            selectedEquippedGemIndex = -1;
            GemData gem = gemBag.gems[index];
            gemDescriptionText.text = GetGemDescription(gem);
            gemNameText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(GetRarityColor(gem.rarity))}>({gem.rarity})</color> {gem.gemName}";

            gemNameText.gameObject.SetActive(true);
            gemDescriptionText.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(true);
            unequipButton.gameObject.SetActive(false);
            destroyButton.gameObject.SetActive(true);

            // Notify GemMergeUI about the gem click
            if (mergeUI.activeSelf)
            {
                GemMergeUI mergeScript = mergeUI.GetComponent<GemMergeUI>();
                mergeScript.OnGemClicked(gem, index);
            }
        }
    }

    private void OnEquippedGemClicked(int index)
    {
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        if (index < equippedBag.gems.Count && equippedBag.gems[index] != null)
        {
            selectedEquippedGemIndex = index;
            selectedGemIndex = -1;
            GemData gem = equippedBag.gems[index];
            gemDescriptionText.text = GetGemDescription(gem);
            gemNameText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(GetRarityColor(gem.rarity))}>({gem.rarity})</color> {gem.gemName}";

            gemNameText.gameObject.SetActive(true);
            gemDescriptionText.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
            unequipButton.gameObject.SetActive(true);
            destroyButton.gameObject.SetActive(true);
        }
    }

    private string GetGemDescription(GemData gem)
    {
        string description = gem.description;

        if (gem is LifeGem lifeGem)
        {
            description += $"\n\n<color=yellow>Life Regen</color>: {lifeGem.lifeRegenPerSecond:F2}\n<color=yellow>Max HP Increase</color>: {lifeGem.maxHpIncrease}";
        }
        else if (gem is PowerGem powerGem)
        {
            description += $"\n\n<color=yellow>Might Increase</color>: {powerGem.attackPowerIncrease * 100:F2}%\n<color=red>Move Speed Decrease</color>: -{powerGem.moveSpeedDecrease * 100:F2}%";
        }
        else if (gem is DefenseGem defenseGem)
        {
            description += $"\n\n<color=yellow>Armor Increase</color>: {defenseGem.armorIncrease:F2}\n<color=red>Attack Speed Decrease</color>: -{defenseGem.attackSpeedDecrease * 100:F2}%";
        }
        else if (gem is SpeedGem speedGem)
        {
            description += $"\n\n<color=yellow>Move Speed Increase</color>: {speedGem.moveSpeedIncrease * 100:F2}%\n<color=red>Max Health Decrease</color>: {speedGem.healthDecrease:F2}";
        }
        else if (gem is LuckGem luckGem)
        {
            description += $"\n\n<color=yellow>Luck Increase</color>: {luckGem.luckIncrease * 100:F2}%\n<color=red>Curse Increase</color>: {luckGem.curseIncrease * 100:F2}%";
        }
        else if (gem is RecoveryGem recoveryGem)
        {
            description += $"\n\n<color=yellow>Recovery Increase</color>: {recoveryGem.recoveryIncrease:F2}\n<color=red>Max Health Decrease</color>: {recoveryGem.maxHealthDecrease:F2}";
        }
        else if (gem is DashGem dashGem)
        {
            description += $"\n\n<color=yellow>Dash Count Increase</color>: {dashGem.dashCountIncrease:F2}\n<color=red>Dash Cooldown Increase</color>: {dashGem.dashCooldownIncrease:F2}";
        }
        else if (gem is HealthGem healthGem)
        {
            description += $"\n\n<color=yellow>Gems HP</color>: {healthGem.currentGemHealth:F2}";
        }

        return description;
    }

    private void EquipSelectedGem()
    {
        GemBag gemBag = GemInventoryManager.Instance.GetGemBag();
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        if (selectedGemIndex >= 0 && selectedGemIndex < gemBag.gems.Count)
        {
            GemData gem = gemBag.gems[selectedGemIndex];
            if (equippedBag.AddGem(gem))
            {
                gemBag.RemoveGem(gem);
                UpdateSlots();
                UpdateEquippedSlots();
                ClearSelectedGem();
                equippedGemStatsUI.UpdateEquippedGemStats();
            }
            else
            {
                ShowNotification("Can't move gem. Bag is full.");
            }
        }
        else
        {
            Debug.Log("Invalid gem index selected for equipping.");
        }
    }

    private void UnequipSelectedGem()
    {
        GemBag gemBag = GemInventoryManager.Instance.GetGemBag();
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        if (selectedEquippedGemIndex >= 0 && selectedEquippedGemIndex < equippedBag.gems.Count)
        {
            GemData gem = equippedBag.gems[selectedEquippedGemIndex];
            if (gemBag.AddGem(gem))
            {
                equippedBag.RemoveGem(gem);
                UpdateSlots();
                UpdateEquippedSlots();
                ClearSelectedGem();
                equippedGemStatsUI.UpdateEquippedGemStats();
            }
            else
            {
                ShowNotification("Can't move gem. Bag is full.");
            }
        }
        else
        {
            Debug.Log("Invalid gem index selected for unequipping.");
        }
    }

    private void DestroySelectedGem()
    {
        GemBag gemBag = GemInventoryManager.Instance.GetGemBag();
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        if (selectedGemIndex >= 0 && selectedGemIndex < gemBag.gems.Count)
        {
            GemData gem = gemBag.gems[selectedGemIndex];
            gemBag.RemoveGem(gem);
            UpdateSlots();
            ClearSelectedGem();
            equippedGemStatsUI.UpdateEquippedGemStats();
        }
        else if (selectedEquippedGemIndex >= 0 && selectedEquippedGemIndex < equippedBag.gems.Count)
        {
            GemData gem = equippedBag.gems[selectedEquippedGemIndex];
            equippedBag.RemoveGem(gem);
            UpdateEquippedSlots();
            ClearSelectedGem();
            equippedGemStatsUI.UpdateEquippedGemStats();
        }
    }

    public void UpdateSlots()
    {
        GemBag gemBag = GemInventoryManager.Instance.GetGemBag();
        for (int i = 0; i < gemImages.Count; i++)
        {
            if (i < gemBag.gems.Count && gemBag.gems[i] != null)
            {
                gemImages[i].sprite = gemBag.gems[i].icon;
                gemImages[i].color = Color.white;
                if (gemBag.gems[i].rarity == GemRarity.Epic)
                {
                    gemImages[i].material = gemBag.gems[i].epicMaterial;
                }
                else if (gemBag.gems[i].rarity == GemRarity.Legendary)
                {
                    gemImages[i].material = gemBag.gems[i].legendaryMaterial;
                }
                else
                {
                    gemImages[i].material = null;
                }
            }
            else
            {
                gemImages[i].sprite = null;
                gemImages[i].color = new Color(0, 0, 0, 0.5f);
                gemImages[i].material = null;
            }
            frameImages[i].enabled = true;
        }
    }

    public void UpdateEquippedSlots()
    {
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        for (int i = 0; i < equippedGemImages.Count; i++)
        {
            if (i < equippedBag.gems.Count && equippedBag.gems[i] != null)
            {
                equippedGemImages[i].sprite = equippedBag.gems[i].icon;
                equippedGemImages[i].color = Color.white;
                if (equippedBag.gems[i].rarity == GemRarity.Epic)
                {
                    equippedGemImages[i].material = equippedBag.gems[i].epicMaterial;
                }
                else if (equippedBag.gems[i].rarity == GemRarity.Legendary)
                {
                    equippedGemImages[i].material = equippedBag.gems[i].legendaryMaterial;
                }
                else
                {
                    equippedGemImages[i].material = null;
                }
            }
            else
            {
                equippedGemImages[i].sprite = null;
                equippedGemImages[i].color = new Color(0, 0, 0, 0.5f);
                equippedGemImages[i].material = null;
            }
        }
    }

    private void ClearSelectedGem()
    {
        selectedGemIndex = -1;
        selectedEquippedGemIndex = -1;
        gemNameText.text = "";
        gemDescriptionText.text = "";
        gemNameText.gameObject.SetActive(false);
        gemDescriptionText.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        destroyButton.gameObject.SetActive(false);
    }

    private Color GetRarityColor(GemRarity rarity)
    {
        switch (rarity)
        {
            case GemRarity.Common:
                return Color.gray;
            case GemRarity.Uncommon:
                return Color.green;
            case GemRarity.Rare:
                return Color.blue;
            case GemRarity.Epic:
                return Color.magenta;
            case GemRarity.Legendary:
                return Color.yellow;
            default:
                return Color.white;
        }
    }

    private void SortGemsByName()
    {
        GemBag gemBag = GemInventoryManager.Instance.GetGemBag();
        gemBag.gems.Sort((gem1, gem2) => string.Compare(gem1.gemName, gem2.gemName));
        UpdateSlots();
    }

    private void OpenMergeUI()
    {
        mergeUI.SetActive(true);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        destroyButton.gameObject.SetActive(false);
        openMergeUIButton.gameObject.SetActive(false);
    }

    private void CloseMergeUI()
    {
        mergeUI.SetActive(false);
        openMergeUIButton.gameObject.SetActive(true);
        // Re-enable equip and destroy buttons only if a gem is selected
        if (selectedGemIndex >= 0)
        {
            equipButton.gameObject.SetActive(true);
            destroyButton.gameObject.SetActive(true);
        }
        else if (selectedEquippedGemIndex >= 0)
        {
            unequipButton.gameObject.SetActive(true);
            destroyButton.gameObject.SetActive(true);
        }
    }

    private void ShowNotification(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(2f);  // Show the message for 2 seconds
        notificationText.gameObject.SetActive(false);
    }
}
