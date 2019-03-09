using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class WeaponItem : Item
{
    /// <summary>
    /// typ zbraně
    /// </summary>
    public WeaponType weaponType;
    /// <summary>
    /// minimální poškození zbraně
    /// </summary>
    public int minDamage;
    /// <summary>
    /// maximální poškození zbraně
    /// </summary>
    public int maxDamage;
    /// <summary>
    /// rychlost útoku zbraně
    /// </summary>
    public int attackSpeed;
    /// <summary>
    /// game object zbraně
    /// </summary>
    public GameObject weaponGameObject;

    public static WeaponItem Generate(Item item)
    {
        //vygenerování náhodného vzoru
        var pattern = WeaponPattern.AllWeaponPatterns.Find(w => (w.lowerItemLevel >= item.itemLevel && w.upperItemLevel <= item.itemLevel));

        if (pattern == null)
        {
            Debug.Log("Neexistuje zbraň s daným item levelem.");
            return null;
        }

        //přiřazení vlastností, které mají všechny předměty společné
        WeaponItem weapon = new WeaponItem();
        weapon.itemLevel = item.itemLevel;
        weapon.rarity = item.rarity;
        weapon.quality = item.quality;
        weapon.itemType = ItemType.Weapon;

        //přiřazení vlastností, které vycházejí ze vzoru
        weapon.attackSpeed = pattern.attackSpeed;
        weapon.sprite = pattern.sprite;
        weapon.name = pattern.name;
        weapon.weaponType = pattern.weaponType;
        weapon.weaponGameObject = pattern.gameObject;
        weapon.minDamage = UnityEngine.Random.Range(pattern.lowerMinDamage, pattern.upperMinDamage + 1);
        weapon.maxDamage = UnityEngine.Random.Range(pattern.lowerMaxDamage, pattern.upperMaxDamage + 1);

        return weapon;
    }

    /// <summary>
    /// Metoda vracející rozsah poškození zbraně (max damage - min damage).
    /// </summary>
    /// <returns>rozsah poškození</returns>
    public int Range()
    {
        return maxDamage - minDamage;
    }
}

