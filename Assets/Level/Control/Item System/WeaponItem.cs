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
    private int minDamage;
    public int MinDamage
    {
        get
        {
            return minDamage + (quality == Quality.C ? ((WeaponPattern)pattern).damageUpgrade : 0);
        }
        set
        {
            minDamage = value;
        }

    }
    /// <summary>
    /// maximální poškození zbraně
    /// </summary>
    private int maxDamage;
    public int MaxDamage
    {
        get {
            return maxDamage + (quality == Quality.C ? ((WeaponPattern)pattern).damageUpgrade : 0);
        }
        set {
            maxDamage = value;
        }

    }
    /// <summary>
    /// rychlost útoku zbraně
    /// </summary>
    public int attackSpeed {
        get {
            return ((WeaponPattern)pattern).attackSpeed + (quality == Quality.C ? ((WeaponPattern)pattern).speedUpgrade : 0);
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

    public WeaponItem(Item item)
    {
        itemType = ItemType.Weapon;
        itemStats = item.itemStats;
        quality = item.quality;

        //přiřazení vlastností, které vycházejí ze vzoru
        pattern = item.pattern;
        MinDamage = ((WeaponItem)item).minDamage;
        MaxDamage = ((WeaponItem)item).maxDamage;
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
            MinDamage = Random.Range(pattern.lowerMinDamage, pattern.upperMinDamage + 1),
            MaxDamage = Random.Range(pattern.lowerMaxDamage, pattern.upperMaxDamage + 1)
        };
        

        return weapon;
    }

    public static WeaponItem Generate(SaveWeapon save)
    {
        WeaponPattern pattern = WeaponPattern.AllWeaponPatterns.Find(x => x.itemName == save.ItemName);
        if (pattern == null) return null;
        //přiřazení vlastností, které jsou uložené
        WeaponItem weapon = new WeaponItem
        {
            itemType = ItemType.Weapon,
            pattern = pattern,
            quality = save.ItemQuality,
            MinDamage =Mathf.Min(save.MinDamage+pattern.lowerMinDamage,pattern.upperMinDamage),
            MaxDamage = Mathf.Min(save.MaxDamage+pattern.upperMaxDamage,pattern.upperMaxDamage),
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
        return MaxDamage - MinDamage;
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
        return MinDamage * (100 + (equip.AllStats["DamageMultiplicative"]) +  GetStat("DamageMultiplicative")) / 100 + equip.AllStats["DamageAdditive"]+GetStat("DamageAdditive");
    }

    public int TotalAttackSpeed(EquipManager equip)
    {
        return attackSpeed + equip.AllStats["AttackSpeed"] + GetStat("AttackSpeed");
    }
}

