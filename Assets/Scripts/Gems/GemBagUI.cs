using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GemBagUI : MonoBehaviour
{
    public GameObject gemSlotPrefab; // The prefab for the gem slots
    public TMP_Text gemNameText; // Reference to the Text component for displaying the gem name with rarity
    public TMP_Text gemDescriptionText; // Reference to the Text component for displaying the gem description
    public Button equipButton; // Reference to the equip button
    public Button unequipButton; // Reference to the unequip button
    public Button destroyButton; // Reference to the destroy button
    public Button sortButton; // Reference to the sort button
    public EquippedGemStatsUI equippedGemStatsUI; // Reference to the EquippedGemStatsUI script

    private Transform equippedGemTransform; // Parent transform for equipped gem slots
    private Transform inventoryGemTransform; // Parent transform for equipped gem slots
    private List<Image> gemImages = new List<Image>(); // Stores references to the gem Image components
    private List<Image> frameImages = new List<Image>(); // Stores references to the frame Image components
    private List<Image> equippedGemImages = new List<Image>(); // Stores references to the equipped gem Image components
    private int selectedGemIndex = -1; // Keeps track of the currently selected gem index
    private int selectedEquippedGemIndex = -1; // Keeps track of the currently selected equipped gem index

    void Start()
    {
        equippedGemTransform = transform.Find("Gem Equipped");
        inventoryGemTransform = transform.Find("Gem Inventory");

        CreateSlots();
        CreateEquippedSlots();
        UpdateSlots();  // Ensure the slots are updated right after creation
        UpdateEquippedSlots(); // Ensure equipped slots are updated right after creation

        // Add listeners to the buttons
        equipButton.onClick.AddListener(EquipSelectedGem);
        unequipButton.onClick.AddListener(UnequipSelectedGem);
        destroyButton.onClick.AddListener(DestroySelectedGem);
        sortButton.onClick.AddListener(SortGemsByName); // Add listener for the sort button

        // Hide the buttons and gem details initially
        ClearSelectedGem();
    }

    void OnEnable()
    {
        UpdateSlots();  // Update slots whenever the UI is enabled
        UpdateEquippedSlots(); // Update equipped slots whenever the UI is enabled
    }

    private void CreateSlots()
    {
        // Clear previous slots if necessary
        foreach (Transform child in inventoryGemTransform)
        {
            Destroy(child.gameObject);
        }
        gemImages.Clear();
        frameImages.Clear();

        // Instantiate slots based on the max capacity of the gem bag
        for (int i = 0; i < GemInventoryManager.Instance.GetGemBag().maxCapacity; i++)
        {
            GameObject slotObj = Instantiate(gemSlotPrefab, inventoryGemTransform);
            Image gemImage = slotObj.GetComponent<Image>();
            Transform frameTransform = slotObj.transform.Find("Frame");
            Image frameImage = frameTransform.GetComponent<Image>();

            gemImages.Add(gemImage);
            frameImages.Add(frameImage);

            int index = i; // Capture the current index for use in the lambda expressions
            gemImage.gameObject.AddComponent<Button>().onClick.AddListener(() => OnGemClicked(index));
        }
    }

    private void CreateEquippedSlots()
    {
        // Clear previous equipped slots if necessary
        foreach (Transform child in equippedGemTransform)
        {
            Destroy(child.gameObject);
        }
        equippedGemImages.Clear();

        // Instantiate slots based on the max capacity of the equipped gem bag
        for (int i = 0; i < GemInventoryManager.Instance.GetEquippedBag().maxCapacity; i++)
        {
            GameObject slotObj = Instantiate(gemSlotPrefab, equippedGemTransform);
            Image gemImage = slotObj.GetComponent<Image>();
            Transform frameTransform = slotObj.transform.Find("Frame");
            Image frameImage = frameTransform.GetComponent<Image>();

            equippedGemImages.Add(gemImage);

            int index = i; // Capture the current index for use in the lambda expressions
            gemImage.gameObject.AddComponent<Button>().onClick.AddListener(() => OnEquippedGemClicked(index));
        }
    }

    private void OnGemClicked(int index)
    {
        GemBag gemBag = GemInventoryManager.Instance.GetGemBag();
        if (index < gemBag.gems.Count && gemBag.gems[index] != null)
        {
            selectedGemIndex = index;
            selectedEquippedGemIndex = -1; // Deselect any equipped gem
            GemData gem = gemBag.gems[index];
            gemDescriptionText.text = GetGemDescription(gem); // Display the gem description
            gemNameText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(GetRarityColor(gem.rarity))}>({gem.rarity})</color> {gem.gemName}";
            
            // Show the buttons and gem details
            gemNameText.gameObject.SetActive(true);
            gemDescriptionText.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(true);
            unequipButton.gameObject.SetActive(false);
            destroyButton.gameObject.SetActive(true);
        }
    }

    private void OnEquippedGemClicked(int index)
    {
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        if (index < equippedBag.gems.Count && equippedBag.gems[index] != null)
        {
            selectedEquippedGemIndex = index;
            selectedGemIndex = -1; // Deselect any gem from the inventory
            GemData gem = equippedBag.gems[index];
            gemDescriptionText.text = GetGemDescription(gem); // Display the gem description
            gemNameText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(GetRarityColor(gem.rarity))}>({gem.rarity})</color> {gem.gemName}";
            
            // Show the buttons and gem details
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
            description += $"\n\n<color=yellow>Move Speed Increase</color>: {speedGem.moveSpeedIncrease:F2}\n<color=red>Max Health Decrease</color>: {speedGem.healthDecrease:F2}";
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
                gemBag.RemoveGem(gem); // Remove the gem from the inventory
                UpdateSlots(); // Refresh the UI to reflect the change
                UpdateEquippedSlots(); // Refresh the equipped gems UI
                ClearSelectedGem(); // Clear the selected gem UI
                equippedGemStatsUI.UpdateEquippedGemStats(); // Update equipped gem stats
            }
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
                equippedBag.RemoveGem(gem); // Remove the gem from the equipped bag
                UpdateSlots(); // Refresh the UI to reflect the change
                UpdateEquippedSlots(); // Refresh the equipped gems UI
                ClearSelectedGem(); // Clear the selected gem UI
                equippedGemStatsUI.UpdateEquippedGemStats(); // Update equipped gem stats
            }
        }
    }

    private void DestroySelectedGem()
    {
        GemBag gemBag = GemInventoryManager.Instance.GetGemBag();
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        if (selectedGemIndex >= 0 && selectedGemIndex < gemBag.gems.Count)
        {
            GemData gem = gemBag.gems[selectedGemIndex];
            gemBag.RemoveGem(gem); // Remove the gem from the inventory
            UpdateSlots(); // Refresh the UI to reflect the change
            ClearSelectedGem(); // Clear the selected gem UI
            equippedGemStatsUI.UpdateEquippedGemStats(); // Update equipped gem stats
        }
        else if (selectedEquippedGemIndex >= 0 && selectedEquippedGemIndex < equippedBag.gems.Count)
        {
            GemData gem = equippedBag.gems[selectedEquippedGemIndex];
            equippedBag.RemoveGem(gem); // Remove the gem from the equipped bag
            UpdateEquippedSlots(); // Refresh the UI to reflect the change
            ClearSelectedGem(); // Clear the selected gem UI
            equippedGemStatsUI.UpdateEquippedGemStats(); // Update equipped gem stats
        }
    }

    public void UpdateSlots()
    {
        GemBag gemBag = GemInventoryManager.Instance.GetGemBag();
        // Make sure each slot represents the state of the gem bag
        for (int i = 0; i < gemImages.Count; i++)
        {
            if (i < gemBag.gems.Count && gemBag.gems[i] != null)
            {
                gemImages[i].sprite = gemBag.gems[i].icon; // Set the gem icon if available
                gemImages[i].color = Color.white; // Ensure full color when a gem is present
            }
            else
            {
                gemImages[i].sprite = null; // Reset the sprite to null for empty slots
                gemImages[i].color = new Color(0, 0, 0, 0.5f); // Semi-transparent for empty slots
            }
            frameImages[i].enabled = true; // Ensure the frame image is always enabled
        }
    }

    public void UpdateEquippedSlots()
    {
        GemBag equippedBag = GemInventoryManager.Instance.GetEquippedBag();
        // Make sure each slot represents the state of the equipped gem bag
        for (int i = 0; i < equippedGemImages.Count; i++)
        {
            if (i < equippedBag.gems.Count && equippedBag.gems[i] != null)
            {
                equippedGemImages[i].sprite = equippedBag.gems[i].icon; // Set the gem icon if available
                equippedGemImages[i].color = Color.white; // Ensure full color when a gem is present
            }
            else
            {
                equippedGemImages[i].sprite = null; // Reset the sprite to null for empty slots
                equippedGemImages[i].color = new Color(0, 0, 0, 0.5f); // Semi-transparent for empty slots
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
        UpdateSlots(); // Refresh the UI to reflect the sorted gems
    }
}
