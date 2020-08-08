using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmorItem : Item
{
    /// <summary>
    /// brnění
    /// </summary>
    public int armor;
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
            armor = Random.Range(pattern.lowerArmor, pattern.upperArmor + 1)
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
            armor = save.Armor,
            itemStats = save.ItemStats,
        };

        return armor;
    }

}
