using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrostCoolDownUI : MonoBehaviour
{
    public Image Image;
    public bool IsIceSlowed = false;
    PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        Image.fillAmount = 0;
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        IceSlowed();
    }

    public void IceSlowed()
    {
        if (playerMovement.IsIceSlowed)
        {
            Image.fillAmount = 1;
            IsIceSlowed = true;
            Debug.Log("IsIceSlowed set to true");
        }
        else
        {
            Image.fillAmount = 0;
            IsIceSlowed = false;
        }
    }
}
    