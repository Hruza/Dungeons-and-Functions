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

    public int AdditionalHP
    {
        get
        {
            return ((ArmorPattern)pattern).additionalHP + (quality == Quality.C ? ((ArmorPattern)pattern).HPUpgrade : 0);
        }
    }

    /// <summary>
    /// zpomalení hráče
    /// </summary>
    public int movementSpeedReduction {
        get {
            return ((ArmorPattern)pattern).movementSpeedReduction + (quality == Quality.C ? ((ArmorPattern)pattern).speedUpgrade : 0); ;
        }
    }

    public ArmorItem() : base()
    {
    }

    public ArmorItem(Item item) : base()
    {
            pattern = item.pattern;
            quality = item.quality;
            itemType = ItemType.Armor;
            itemStats = item.itemStats;
        Armor = ((ArmorItem)item).armor;
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

        return armor;
    }

    public static ArmorItem Generate(SaveArmor save)
    {
        ArmorPattern pattern = ArmorPattern.AllArmorPatterns.Find(x => x.itemName == save.ItemName);

        if (pattern == null) return null;
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
