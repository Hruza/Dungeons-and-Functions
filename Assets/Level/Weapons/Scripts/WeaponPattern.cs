using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "WeaponPattern", menuName = "Weapon Pattern")]
public class WeaponPattern : ItemPattern
{
    /// <summary>
    /// typ zbraně
    /// </summary>
    public WeaponType weaponType;

    public Damager.DamageType damageType;
    /// <summary>
    /// dolní hranice minimálnoho poškození zbraně
    /// </summary>
    public int lowerMinDamage;
    /// <summary>
    /// horní hranice minimálnoho poškození zbraně
    /// </summary>
    public int upperMinDamage;
    /// <summary>
    /// dolní hranice maximálního poškození zbraně
    /// </summary>
    public int lowerMaxDamage;
    /// <summary>
    /// horní hranice maximálního poškození zbraně
    /// </summary>
    public int upperMaxDamage;
    /// <summary>
    /// rychlost útoku zbraně
    /// </summary>
    public int attackSpeed;

    /// <summary>
    /// game object zbraně
    /// </summary>
    public GameObject gameObject;

    [Header("Quality upgrade")]
    public int damageUpgrade=2;

    public int speedUpgrade=2;


    /// <summary>
    /// databáze všech možných zbraní
    /// </summary>
    public static List<WeaponPattern> AllWeaponPatterns;

    public override ItemType Type()
    {
        return ItemType.Weapon;
    }
    /*new List<WeaponPattern>1
    {
        new WeaponPattern
        {
            name = "ItegralSword",
            weaponType = WeaponType.Melee,
            lowerItemLevel = 1,
            upperItemLevel = 1000,
            lowerMinDamage = 1,
            upperMinDamage = 4,
            lowerMaxDamage = 5,
            upperMaxDamage = 10,
            attackSpeed = 1
        }
    };*/
}
