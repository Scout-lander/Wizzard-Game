using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDamageKillsDps : MonoBehaviour
{
    [Header("Kills/Damage")]
    GameManager gameManager;

    public int killCount = 0;
    public float totalDamageDone = 0;
    public float dps;

    public TMP_Text kills;
    public TMP_Text damage;
    public TMP_Text DPS;

    public void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        StartCoroutine(UpdateKillCountTextCoroutine()); // Start the coroutine to update kill count text
        StartCoroutine(UpdateDamageTextCoroutine()); // Start the coroutine to update kill count text
        StartCoroutine(UpdateDPSTextCoroutine()); // Start coroutine to update DPS text
    }

    public void IncrementKillCount()
    {
        killCount++;
        // Optionally, you can update UI or perform other actions related to the kill count here
    }

    // Method to get the current kill count
    public int GetKillCount()
    {
        return killCount;
    }
    
    public void IncrementTotalDamageDone(float dmg)
    {
        totalDamageDone += dmg;
    }

    public string GetTotalDamageDoneFormatted()
    {
        if (totalDamageDone >= 1000)
        {
            float damageInK = totalDamageDone / 1000f;
            return damageInK.ToString("0.00") + "k";
        }
        else
        {
            return totalDamageDone.ToString("0");
        }
    }

    public void UpdateDPS()
    {
        if (gameManager.stopwatchTime > 0)
        {
            dps = totalDamageDone / gameManager.stopwatchTime;
            DPS.text = "" + dps.ToString("F2"); // Display DPS with 2 decimal places
        }
    }

    // Method to update the kill count text
    IEnumerator UpdateDamageTextCoroutine()
    {
        while (true) // Infinite loop to update the text every second
        {
            yield return new WaitForSeconds(1f); // Wait for one second

            if (totalDamageDone > 0) // Check if Damage is greater then 0
            {
                // Update the kill count text with the current kill count
                damage.text = "" + GetTotalDamageDoneFormatted().ToString();
            }
        }
    }

    IEnumerator UpdateDPSTextCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Wait for one second

            UpdateDPS(); // Update DPS text
        }
    }

    IEnumerator UpdateKillCountTextCoroutine()
    {
        while (true) // Infinite loop to update the text every second
        {
            yield return new WaitForSeconds(1f); // Wait for one second

            if (killCount > 0) // Check if Kill Count is Greater then 0
            {
                // Update the kill count text with the current kill count
                kills.text = "" + GetKillCount().ToString();
            }
        }
    }
}
