using UnityEngine;

public abstract class WeaponEffect : MonoBehaviour
{
    [HideInInspector] public PlayerStats owner;
    [HideInInspector] public Weapon weapon;

    public PlayerStats Owner { get { return owner; } }

    public float GetDamage()
    {

        Weapon.Stats weaponStats = weapon.GetStats();
        float damage = weapon.GetDamage();

        // Add the calculated damage to the weapon's damageDone field
        //weaponStats.damageDone += damage;

        return damage;

    }
}