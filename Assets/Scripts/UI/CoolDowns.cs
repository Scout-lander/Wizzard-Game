using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoolDowns : MonoBehaviour
{
    // CoolDown Images
    public Image DashImage;
    public TMP_Text dashCount;

    // Bools
    bool isDash = false;

    PlayerMovement playerMovement;
    PlayerStats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        DashImage.fillAmount = 0;

        playerMovement = FindObjectOfType<PlayerMovement>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    void LateUpdate()
    {
        DashUI();
        dashCount.text = playerMovement.currentDashes.ToString();
    }

    public void DashUI()
    {
        if (!isDash && playerMovement.currentDashes < playerStats.ActualStats.maxDashes)
        {
            isDash = true;
            DashImage.fillAmount = 1;
            StartCoroutine(playerMovement.ReplenishDash());
        }

        if (isDash)
        {
            DashImage.fillAmount -= 1 / playerStats.ActualStats.dashCooldown * Time.deltaTime;

            if (DashImage.fillAmount <= 0)
            {
                DashImage.fillAmount = 0;
                isDash = false;
            }
        }
    }
}
