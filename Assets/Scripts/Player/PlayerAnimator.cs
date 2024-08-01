using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // References
    private Animator am;
    private PlayerMovement pm;
    private Transform tr;

    [SerializeField]
    private GameObject otherObjectToFlip;

    void Start()
    {
        am = GetComponentInChildren<Animator>();
        pm = GetComponent<PlayerMovement>();
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        if (pm.moveDir.x != 0 || pm.moveDir.y != 0)
        {
            am.SetBool("Move", true);
            am.SetInteger("State", 2);

            SpriteDirectionChecker();
        }
        else
        {
            am.SetBool("Move", false);
            am.SetInteger("State", 0);
        }
    }

    void SpriteDirectionChecker()
    {
        if (pm.lastHorizontalVector < 0)
        {
            tr.localScale = new Vector3(-Mathf.Abs(tr.localScale.x), tr.localScale.y, tr.localScale.z);
            if (otherObjectToFlip != null)
            {
                otherObjectToFlip.transform.localScale = new Vector3(-Mathf.Abs(otherObjectToFlip.transform.localScale.x), otherObjectToFlip.transform.localScale.y, otherObjectToFlip.transform.localScale.z);
            }
        }
        else if (pm.lastHorizontalVector > 0)
        {
            tr.localScale = new Vector3(Mathf.Abs(tr.localScale.x), tr.localScale.y, tr.localScale.z);
            if (otherObjectToFlip != null)
            {
                otherObjectToFlip.transform.localScale = new Vector3(Mathf.Abs(otherObjectToFlip.transform.localScale.x), otherObjectToFlip.transform.localScale.y, otherObjectToFlip.transform.localScale.z);
            }
        }
    }
}
