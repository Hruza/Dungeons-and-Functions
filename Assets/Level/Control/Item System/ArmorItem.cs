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
    public int movementSpeedReduction;

    public ArmorItem() : base()
    {
    }

    public ArmorItem(Item item)
    {
        itemLevel = item.itemLevel;
        rarity = item.rarity;
        quality = item.quality;
        itemType = ItemType.Armor;
    }

    public static ArmorItem Generate(Item item)
    {
        //vygenerování náhodného vzoru
        ArmorPattern.AllArmorPatterns = ArmorPattern.AllArmorPatterns.Shuffle();
        var pattern = ArmorPattern.AllArmorPatterns.Find(w => (w.lowerItemLevel <= item.itemLevel && w.upperItemLevel >= item.itemLevel));

        if (pattern == null)
        {
            Debug.Log("Neexistuje brnění s daným item levelem.");
            return null;
        }

        ArmorItem armor = Generate(item, pattern);

        return armor;
    }

    public static ArmorItem Generate(Item item, ArmorPattern pattern,bool noStats=false)
    {
        //přiřazení vlastností, které mají všechny předměty společné
        ArmorItem armor = new ArmorItem
        {
            itemLevel = item.itemLevel,
            rarity = item.rarity,
            quality = item.quality,
            itemType = ItemType.Armor,
            itemStats = new Stat[0],

            //přiřazení vlastností, které vycházejí ze vzoru
            itemName = pattern.name,
            sprite = pattern.sprite,
            movementSpeedReduction = pattern.movementSpeedReduction,
            armor = item.itemLevel * pattern.armorIncrementPerLevel + Random.Range(pattern.lowerArmor, pattern.upperArmor + 1)
        };

        //vylepšení brnění v případě, že má vyšší kvalitu
        if (armor.quality == Quality.C)
        {
            armor.Upgrade();
            armor.quality = Quality.C;
        }

        //vylepšení brnění v případě, že má vyšší raritu
        if (armor.rarity == Rarity.Rare)
            armor.armor = (int)(armor.armor * rarityUpgrade);
        if (armor.rarity == Rarity.Unique)
            armor.armor = (int)(armor.armor * qualityUpgrade * qualityUpgrade);

        if(!noStats) armor.GenerateStats();

        return armor;
    }

    /// <summary>
    /// Metoda sloužící pro vylepšování brnění (zvyšuje kvalitu).
    /// </summary>
    public void Upgrade()
    {
        if (quality == Quality.Csharp)
        {
            Debug.Log("Pokoušíš se vylepšit předmět, který už vylepšit nelze.");
            return;
        }
        quality = Extensions.NextElement(quality);

        armor = (int)(armor * qualityUpgrade);
    }
}
