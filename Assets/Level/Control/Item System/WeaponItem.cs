using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class WeaponItem : Item
{
    /// <summary>
    /// typ zbraně
    /// </summary>
    public WeaponType weaponType
    {
        get
        {
            return ((WeaponPattern)pattern).weaponType;
        }
    }
    /// <summary>
    /// typ poškození zbraně
    /// </summary>
    public Damager.DamageType damageType
    {
        get
        {
            return ((WeaponPattern)pattern).damageType;
        }
    }
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
    public int attackSpeed {
        get {
            return ((WeaponPattern)pattern).attackSpeed;
        }
    }
    /// <summary>
    /// game object zbraně
    /// </summary>
    public GameObject weaponGameObject
    {
        get
        {
            return ((WeaponPattern)pattern).gameObject;
        }
    }

    public WeaponItem() : base()
    {
    }

    public static WeaponItem Generate(WeaponPattern pattern,bool noStats=false) {
        //přiřazení vlastností, které mají všechny předměty společné
        WeaponItem weapon = new WeaponItem
        {
            itemType = ItemType.Weapon,
            itemStats = new Stat[0],
            quality = Quality.Basic,

            //přiřazení vlastností, které vycházejí ze vzoru
            pattern = pattern,
            minDamage = Random.Range(pattern.lowerMinDamage, pattern.upperMinDamage + 1),
            maxDamage = Random.Range(pattern.lowerMaxDamage, pattern.upperMaxDamage + 1)
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
            pattern = pattern,
            quality = save.ItemQuality,
            minDamage = save.MinDamage,
            maxDamage = save.MaxDamage,
            itemStats = save.ItemStats,

            //přiřazení vlastností, které vycházejí ze vzoru
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

