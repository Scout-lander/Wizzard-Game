using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCoolDownUI : MonoBehaviour
{
    public Image Image;
    bool isDash = false;
    PlayerStats player;
    PlayerMovement playerMovement; // Reference to PlayerMovement script
    

    // Start is called before the first frame update
    void Start()
    {
        Image.fillAmount = 0;
        //player = GetComponent<PlayerStats>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Dash();
    }

    void Dash()
    {
        if (!isDash && playerMovement.isDashing)
        {
            isDash = true;
            Image.fillAmount = 1;
        }

        if (isDash)
        {
            Image.fillAmount -= 1 / playerMovement.player.Stats.dashCooldown * Time.deltaTime;

            if (Image.fillAmount <= 0)
            {
                Image.fillAmount = 0;
                isDash = false;
            }
        }
    }
}
