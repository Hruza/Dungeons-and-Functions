using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/// <summary>
/// Trida uchovavajici vsechny potrebne informace o itemu pro jeho ulozeni
/// </summary>
[System.Serializable]
public class SaveItem
{
    public string ItemName { get; protected set; }

    public Quality ItemQuality { get; protected set; }

    public Stat[] ItemStats { get; protected set; }
}

[System.Serializable]
public class SaveWeapon : SaveItem
{
    public int MinDamage { get; protected set; }
    public int MaxDamage { get; protected set; }


    public SaveWeapon(WeaponItem weaponItem)
    {
        ItemName = weaponItem.pattern.itemName;
        ItemQuality = weaponItem.quality;
        ItemStats = weaponItem.itemStats;
        MinDamage = weaponItem.MinDamage-((WeaponPattern)weaponItem.pattern).lowerMinDamage;
        MaxDamage = weaponItem.MaxDamage-((WeaponPattern)weaponItem.pattern).lowerMaxDamage;
    }

    public WeaponItem GetItem()
    {
        WeaponItem weaponItem = WeaponItem.Generate(this);
        return weaponItem;
    }
}

[System.Serializable]
public class SaveArmor : SaveItem
{
    public int Armor { get; protected set; }

    public SaveArmor(ArmorItem armorItem)
    {
        ItemName = armorItem.pattern.itemName;
        ItemQuality = armorItem.quality;
        ItemStats = armorItem.itemStats;
        Armor = armorItem.Armor-((ArmorPattern)armorItem.pattern).lowerArmor;
    }

    public ArmorItem GetItem()
    {
        ArmorItem armorItem = ArmorItem.Generate(this);
        return armorItem;
    }

}