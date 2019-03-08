using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// třída spravující vybavení hráče
/// </summary>
public class EquipManager
{
    /// <summary>
    /// seznam všech předmětů (NE ZBRANÍ), které má hráč momentálně na sobě
    /// </summary>
    public static List<Item> EquippedItems { get; private set; }
    /// <summary>
    /// seznam všech zbraní, které hráč může využívat
    /// </summary>
    public static List<WeaponItem> EquippedWeapons { get; private set; }
    /// <summary>
    /// seznam a hodnota každého bonusového statu, který má hráč na sobě
    /// </summary>
    public Dictionary<string, int> AllStats;
    
    public EquipManager()
    {
        EquippedItems = new List<Item>();
        EquippedWeapons = new List<WeaponItem>();
        AllStats = new Dictionary<string, int>();

        foreach (StatPattern statPattern in StatPattern.AllStatPatterns)
        {
            AllStats.Add(statPattern.name, 0);
        }

        CountAllStats();
    }

    /// <summary>
    /// metoda, která projde všechny předměty a přepočítá všechny vlastnosti
    /// </summary>
    private void CountAllStats()
    {
        foreach (Item item in EquippedItems)
            AddStats(item.itemStats);

        foreach (WeaponItem weapon in EquippedWeapons)
            AddStats(weapon.itemStats);
    }

    /// <summary>
    /// přičte hodnoty statů k AllStats
    /// </summary>
    /// <param name="stats">seznam statů</param>
    private void AddStats(Stat[] stats)
    {
        foreach (Stat stat in stats)
            AllStats[stat.name] += stat.value;
    }

    /// <summary>
    /// odečte hodnoty statů od AllStats
    /// </summary>
    /// <param name="stats">seznam statů</param>
    private void DeductStats(Stat[] stats)
    {
        foreach (Stat stat in stats)
            AllStats[stat.name] -= stat.value;
    }
    
    /// <summary>
    /// přidá předmět (NE ZBRAŇ) hráči a přepočítá staty
    /// </summary>
    /// <param name="item">přidávaný předmět</param>
    public void EquipItem(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            Debug.Log("Máš tu menší fuckup, pro přidání předmětu hráči použij EquipWeapon.");
            return;
        }

        //odebrání starého předmětu (pokud existuje)
        var oldItem = EquippedItems.Find(i => i.itemType == item.itemType);
        if (oldItem != null)
        {
            DeductStats(oldItem.itemStats);
            EquippedItems.Remove(oldItem);
        }

        //přidání nové předmětu
        AddStats(item.itemStats);
        EquippedItems.Add(item);
    }

    /// <summary>
    /// Vymění zbraň hráči.
    /// </summary>
    /// <param name="newWeapon">nová zbraň</param>
    /// <param name="oldWeapon">stará zbraň, která má být odebrána</param>
    public void EquipWeapon(WeaponItem newWeapon, WeaponItem oldWeapon)
    {
        //odebrání staré zbraně
        if (oldWeapon != null)
        {
            DeductStats(oldWeapon.itemStats);
            EquippedWeapons.Remove(oldWeapon);
        }

        //přidání nové zbraně
        AddStats(newWeapon.itemStats);
        EquippedWeapons.Add(newWeapon);
    }

    /// <summary>
    /// Přidá zbraň hráči.
    /// </summary>
    /// <param name="newWeapon">nová zbraň, která má být přidána</param>
    public void EquipWeapon(WeaponItem newWeapon)
    {
        //přidání nové zbraně
        AddStats(newWeapon.itemStats);
        EquippedWeapons.Add(newWeapon);
    }

    /// <summary>
    /// Vypočítá nejmenší možné poškození, které hráč může udělit v závislosti na zbrani.
    /// </summary>
    /// <param name="weapon">Index zbraně, ze které má být vypočítáno nejmenší možnéé poškození.</param>
    /// <returns>nejmenší možné poškození</returns>
    public int TotalMinDamage(int weaponIndex)
    {
        return EquippedWeapons[weaponIndex].minDamage * (100 + AllStats["DamageMultiplicative"]) / 100 + AllStats["DamageAdditive"];
    }
}
