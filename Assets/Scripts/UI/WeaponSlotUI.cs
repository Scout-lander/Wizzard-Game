using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image weaponIcon;

    public void SetWeapon(WeaponData weapon)
    {
        if (weapon != null)
        {
            weaponIcon.sprite = weapon.icon;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
