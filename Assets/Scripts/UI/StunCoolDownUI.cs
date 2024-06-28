using UnityEngine;
using UnityEngine.UI;

public class StunCoolDownDownUI : MonoBehaviour
{
    public Image Image;
    bool isStunned = false;
    PlayerMovement playerMovement; // Reference to PlayerMovement script

    // Start is called before the first frame update
    void Start()
    {
        Image.fillAmount = 0;
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Stunned();
    }

    public void Stunned()
    {
        if (playerMovement.isStunned)
        {
            Image.fillAmount = 1;
            isStunned = true;
            Debug.Log("IsStunned set to true");
        }
        else
        {
            Image.fillAmount = 0;
            isStunned = false;
        }
    }
}
