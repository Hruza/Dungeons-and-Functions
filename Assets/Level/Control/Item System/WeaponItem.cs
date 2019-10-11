using System.Collections;
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

    public static WeaponItem Generate(SaveWeapon save)
    {
        WeaponPattern pattern = WeaponPattern.AllWeaponPatterns.Find(x => x.name == save.ItemName);
        //přiřazení vlastností, které jsou uložené
        WeaponItem weapon = new WeaponItem
        {
            itemType = ItemType.Weapon,
            itemLevel = save.ItemLevel,
            rarity = save.ItemRarity,
            quality = save.ItemQuality,
            minDamage = save.MinDamage,
            maxDamage = save.MaxDamage,
            itemStats = save.ItemStats,

            //přiřazení vlastností, které vycházejí ze vzoru
            attackSpeed = pattern.attackSpeed,
            sprite = pattern.sprite,
            itemName = pattern.name,
            weaponType = pattern.weaponType,
            weaponGameObject = pattern.gameObject
        };

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

    public int GetStat(string name)
    {
        int value = 0;
        foreach (Stat stat in itemStats)
        {
            if (stat.name == name) value += stat.value;
        }
        return value;
    }

    public int TotalMinDamage( EquipManager equip)
    {
        return minDamage * (100 + (equip.AllStats["DamageMultiplicative"]) +  GetStat("DamageMultiplicative")) / 100 + equip.AllStats["DamageAdditive"]+GetStat("DamageAdditive");
    }

    public int TotalAttackSpeed(EquipManager equip)
    {
        return attackSpeed + equip.AllStats["AttackSpeed"] + GetStat("AttackSpeed");
    }
}

