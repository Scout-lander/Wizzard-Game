using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DashCoolDownUI : MonoBehaviour
{
    public Image cooldownImage;
    private bool isDashActive = false;
    private PlayerMovement playerMovement;

    void Start()
    {
        cooldownImage.fillAmount = 0;
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        UpdateDashCooldown();
    }

    void UpdateDashCooldown()
    {
        if (playerMovement == null)
        {
            Debug.LogWarning("PlayerMovement script not found!");
            return;
        }

        if (!isDashActive && playerMovement.isDashing)
        {
            isDashActive = true;
            cooldownImage.fillAmount = 1;
        }

        if (isDashActive)
        {
            PlayerStats playerStats = playerMovement.GetPlayerStats();
            if (playerStats != null)
            {
                cooldownImage.fillAmount -= 1 / playerStats.ActualStats.dashCooldown * Time.deltaTime;
            }

            if (cooldownImage.fillAmount <= 0)
            {
                cooldownImage.fillAmount = 0;
                isDashActive = false;
            }
        }
    }
}
