using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmorItem : Item
{
    /// <summary>
    /// brnění
    /// </summary>
    private int armor;
    public int Armor {
        get {
            return armor + (quality == Quality.C ? ((ArmorPattern)pattern).armorUpgrade : 0);
        }
        set {
            armor = value;
        }
    }
    /// <summary>
    /// zpomalení hráče
    /// </summary>
    public int movementSpeedReduction {
        get {
            return ((ArmorPattern)pattern).movementSpeedReduction;
        }
    }

    public ArmorItem() : base()
    {
    }

    public static ArmorItem Generate( ArmorPattern pattern,bool noStats=false)
    {
        //přiřazení vlastností, které mají všechny předměty společné
        ArmorItem armor = new ArmorItem
        {
            pattern=pattern,
            quality = Quality.Basic,
            itemType = ItemType.Armor,
            itemStats = new Stat[0],
            Armor = Random.Range(pattern.lowerArmor, pattern.upperArmor + 1)
        };

        if(!noStats) armor.GenerateStats();

        return armor;
    }

    public static ArmorItem Generate(SaveArmor save)
    {
        ArmorPattern pattern = ArmorPattern.AllArmorPatterns.Find(x => x.name == save.ItemName);
        //přiřazení vlastností, které jsou uložené
        ArmorItem armor = new ArmorItem
        {
            pattern = pattern,
            itemType = ItemType.Armor,
            quality = save.ItemQuality,
            Armor = Mathf.Min(save.Armor+pattern.lowerArmor,pattern.upperArmor),
            itemStats = save.ItemStats,
        };

        return armor;
    }

}
