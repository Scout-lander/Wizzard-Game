using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class WeaponUIDisplay : MonoBehaviour
{
    public GameObject slotTemplate;
    public PlayerInventory inventory;

    Dictionary<WeaponData, float> trackedDamageTotals = new Dictionary<WeaponData, float>();
    Dictionary<WeaponData, float> weaponCollectionTimes = new Dictionary<WeaponData, float>();
    Dictionary<WeaponData, float> weaponDPS = new Dictionary<WeaponData, float>();
    Dictionary<WeaponData, int> weaponLevels = new Dictionary<WeaponData, int>();

    public List<Transform> weaponSlots = new List<Transform>();

    public Button sortByNameButton;
    public Button sortByDamageButton;
    public Button sortByTimeButton;
    public Button sortByDPSButton;

    private void Reset()
    {
        if (transform.childCount > 0)
            slotTemplate = transform.GetChild(0).gameObject;
        inventory = FindObjectOfType<PlayerInventory>();
    }

    void Start()
    {
        sortByNameButton.onClick.AddListener(() => SortWeapons("name"));
        sortByDamageButton.onClick.AddListener(() => SortWeapons("damage"));
        sortByTimeButton.onClick.AddListener(() => SortWeapons("time"));
        sortByDPSButton.onClick.AddListener(() => SortWeapons("dps"));
    }

    void OnEnable()
    {
        UpdateWeaponDictionaries();
        SortWeapons("time");
    }

    void UpdateWeaponDictionaries()
    {
        foreach (PlayerInventory.Slot s in inventory.weaponSlots)
        {
            Weapon w = (Weapon)s.item;
            if (w)
            {
                WeaponData weaponData = (WeaponData)w.data;
                trackedDamageTotals[weaponData] = w.damageTotal;
                weaponCollectionTimes[weaponData] = w.WeaponTime;
                weaponDPS[weaponData] = w.WeaponDPS;
                weaponLevels[weaponData] = w.currentLevel; // Store the weapon level
            }
        }
    }

    void SortWeapons(string criterion)
    {
        List<WeaponData> sortedWeapons = null;

        switch (criterion)
        {
            case "name":
                sortedWeapons = trackedDamageTotals.Keys.OrderBy(key => key.weaponName).ToList();
                break;
            case "damage":
                sortedWeapons = trackedDamageTotals.Keys.OrderByDescending(key => trackedDamageTotals[key]).ToList();
                break;
            case "time":
                sortedWeapons = weaponCollectionTimes.Keys.OrderBy(key => weaponCollectionTimes[key]).ToList();
                break;
            case "dps":
                sortedWeapons = weaponDPS.Keys.OrderByDescending(key => weaponDPS[key]).ToList();
                break;
        }

        UpdateWeaponSlots(sortedWeapons);
    }

    void UpdateWeaponSlots(List<WeaponData> sortedWeapons)
    {
        foreach (var slot in weaponSlots)
        {
            Image[] imgs = slot.GetComponentsInChildren<Image>();
            foreach (Image img in imgs) img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
            TMP_Text[] texts = slot.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text txt in texts) txt.enabled = false;
        }

        int slotIndex = 0;
        foreach (WeaponData weaponData in sortedWeapons)
        {
            if (slotIndex < weaponSlots.Count)
            {
                Transform slot = weaponSlots[slotIndex];

                Image weaponIcon = slot.Find("WeaponIcon").GetComponent<Image>();
                TMP_Text weaponNameText = slot.Find("WeaponNameText").GetComponent<TMP_Text>();
                TMP_Text weaponDamageText = slot.Find("WeaponDamageText").GetComponent<TMP_Text>();
                TMP_Text weaponTimeText = slot.Find("WeaponTimeText").GetComponent<TMP_Text>();
                TMP_Text weaponDPSText = slot.Find("WeaponDPSText").GetComponent<TMP_Text>();
                TMP_Text weaponLevelText = slot.Find("WeaponLevelText").GetComponent<TMP_Text>();

                Image[] imgs = slot.GetComponentsInChildren<Image>();
                foreach (Image img in imgs) img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
                TMP_Text[] texts = slot.GetComponentsInChildren<TMP_Text>();
                foreach (TMP_Text txt in texts) txt.enabled = true;

                weaponIcon.sprite = weaponData.icon;
                weaponNameText.text = weaponData.weaponName;

                if (trackedDamageTotals.TryGetValue(weaponData, out float damageTotal))
                {
                    weaponDamageText.text = damageTotal.ToString("0.00");
                }

                if (weaponCollectionTimes.TryGetValue(weaponData, out float weaponTime))
                {
                    int minutes = Mathf.FloorToInt(weaponTime / 60);
                    int seconds = Mathf.FloorToInt(weaponTime % 60);
                    weaponTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                }

                if (weaponDPS.TryGetValue(weaponData, out float dps))
                {
                    weaponDPSText.text = (dps == 0 || float.IsNaN(dps)) ? "0" : dps.ToString("0.00");
                }

                if (weaponLevels.TryGetValue(weaponData, out int level))
                {
                    weaponLevelText.text = "" + level;
                }

                slotIndex++;
            }
        }
    }
}
