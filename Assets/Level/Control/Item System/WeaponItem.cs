﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

    public WeaponItem() : base()
    {
    }

    public static WeaponItem Generate(Item item)
    {
        //vygenerování náhodného vzoru
        WeaponPattern.AllWeaponPatterns = WeaponPattern.AllWeaponPatterns.Shuffle();
        var pattern = WeaponPattern.AllWeaponPatterns.Find(w => (w.lowerItemLevel <= item.itemLevel && w.upperItemLevel >= item.itemLevel));

        if (pattern == null)
        {
            Debug.Log("Neexistuje zbraň s daným item levelem.");
            return null;
        }
        else
        {
            return Generate(item, pattern);
        }
    }

    public static WeaponItem Generate(Item item,WeaponPattern pattern,bool noStats=false) { 
        //přiřazení vlastností, které mají všechny předměty společné
        WeaponItem weapon = new WeaponItem
        {
            itemLevel = item.itemLevel,
            rarity = item.rarity,
            quality = item.quality,
            itemType = ItemType.Weapon,
            itemStats = new Stat[0],

            //přiřazení vlastností, které vycházejí ze vzoru
            attackSpeed = pattern.attackSpeed,
            sprite = pattern.sprite,
            itemName = pattern.name,
            weaponType = pattern.weaponType,
            weaponGameObject = pattern.gameObject,
            minDamage = item.itemLevel * pattern.damageIncrementPerLevel + Random.Range(pattern.lowerMinDamage, pattern.upperMinDamage + 1),
            maxDamage = item.itemLevel * pattern.damageIncrementPerLevel + Random.Range(pattern.lowerMaxDamage, pattern.upperMaxDamage + 1)
        };
        
        if(!noStats)weapon.GenerateStats();

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
