using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// všechny možné typy předmětů
/// </summary>
public enum ItemType { Armor, Weapon };
/// <summary>
/// všechny možné typy zbraní
/// </summary>
public enum WeaponType { Melee, Ranged };
/// <summary>
/// možné kvality předmětů
/// </summary>
public enum Quality { Basic, C, Cplusplus, Csharp };

/// <summary>
/// možné rarity zbraní
/// </summary>
public enum Rarity { Common, Rare, Unique };

/// <summary>
/// Třída reprezentující předmět, který může hráč získat.
/// </summary>
public class Item : ScriptableObject
{
    /// <summary>
    /// úroveň předmětu
    /// </summary>
    public int itemLevel;
    /// <summary>
    /// jméno předmětu
    /// </summary>
    public string name;
    /// <summary>
    /// kvalita předmětu
    /// </summary>
    public Quality quality;
    /// <summary>
    /// rarita předmětu
    /// </summary>
    public Rarity rarity;
    /// <summary>
    /// typ itemu
    /// </summary>
    public ItemType itemType;
    /// <summary>
    /// seznam všech bpnusových statů, které má předmět
    /// </summary>
    /// <summary>
    /// sprite itemu
    /// </summary>
    public Sprite sprite;

    public Stat[] itemStats;

    /// <summary>
    /// bezparametrický kontruktor (nic neudělá)
    /// </summary>
    public Item()
    {
    }

    /// <summary>
    /// kontruktor vytvářející generický item (obsahuje pouze úroveň, kvalitu a raritu předmětu)
    /// </summary>
    /// <param name="itemLevel"></param>
    public Item(int itemLevel)
    {
        this.itemLevel = itemLevel;
        name = "GenericItem";
        quality = Probability.RandomQuality();
        rarity = Probability.RandomRarity();
    }

    /// <summary>
    /// Delegát obsahující metodu sloužící pro vygenerování předmětu.
    /// </summary>
    /// <param name="item">základ předmětu</param>
    /// <returns>vygenerovaný předmět</returns>
    private delegate Item GeneratingMethods(Item item);

    /// <summary>
    /// statická metoda sloužící pro generování náhodného itemu
    /// </summary>
    /// <param name="itemLevel">level vygenerovaného itemu (stejný, jako level monstra, ze kterého dropnul)</param>
    /// <returns>vygenerovaný item</returns>
    public static Item Generate(int itemLevel)
    {
        Item item = new Item(itemLevel);

        //Seznam všech metod, které slouří pro generovnání náhodných předmětů.
        var listOfMethods = new List<GeneratingMethods>
        {
            WeaponItem.Generate,
            ArmorItem.Generate
        };

        //Zavolání náhodné metody, která vrátí náhodný předmět.
        item = listOfMethods[UnityEngine.Random.Range(0, listOfMethods.Count)](item);

        return item;
    }
}

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

    public static ArmorItem Generate(Item item)
    {
        //vygenerování náhodného vzoru
        var pattern = ArmorPattern.AllArmorPatterns.Find(w => (w.lowerItemLevel >= item.itemLevel && w.upperItemLevel <= item.itemLevel));

        if (pattern == null)
        {
            Debug.Log("Neexistuje zbraň s daným item levelem.");
            return null;
        }

        //přiřazení vlastností, které mají všechny předměty společné
        ArmorItem armor = new ArmorItem();
        armor.itemLevel = item.itemLevel;
        armor.rarity = item.rarity;
        armor.quality = item.quality;
        armor.itemType = ItemType.Weapon;

        //přiřazení vlastností, které vycházejí ze vzoru
        armor.name = pattern.name;
        armor.movementSpeedReduction = pattern.movementSpeedReduction;
        armor.armor = UnityEngine.Random.Range(pattern.lowerArmor, pattern.upperArmor + 1);

        if (armor.quality > Quality.Basic)
            Debug.Log("a");

        return armor;
    }
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
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

    public static WeaponItem Generate(Item item)
    {
        //vygenerování náhodného vzoru
        var pattern = WeaponPattern.AllWeaponPatterns.Find(w => (w.lowerItemLevel >= item.itemLevel && w.upperItemLevel <= item.itemLevel));

        if (pattern == null)
        {
            Debug.Log("Neexistuje zbraň s daným item levelem.");
            return null;
        }

        //přiřazení vlastností, které mají všechny předměty společné
        WeaponItem weapon = new WeaponItem();
        weapon.itemLevel = item.itemLevel;
        weapon.rarity = item.rarity;
        weapon.quality = item.quality;
        weapon.itemType = ItemType.Weapon;

        //přiřazení vlastností, které vycházejí ze vzoru
        weapon.attackSpeed = pattern.attackSpeed;
        weapon.sprite = pattern.sprite;
        weapon.name = pattern.name;
        weapon.weaponType = pattern.weaponType;
        weapon.weaponGameObject = pattern.gameObject;
        weapon.minDamage = UnityEngine.Random.Range(pattern.lowerMinDamage, pattern.upperMinDamage + 1);
        weapon.maxDamage = UnityEngine.Random.Range(pattern.lowerMaxDamage, pattern.upperMaxDamage + 1);

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

/// <summary>
/// Statická třída mající na starosti generování náhodných rarit a kvalit. Obsahuje i jednotilé pravděpodobnosti.
/// </summary>
public static class Probability
{
    /// <summary>
    /// pravděpodobnosti, s jakými mohou být jednotlivé kvality vygenerovány
    /// </summary>
    private static Dictionary<Quality, double> qualityProbabilities = new Dictionary<Quality, double>
    {
        {Quality.Basic, 0.9 },
        {Quality.C, 0.1 }
    };

    /// <summary>
    /// pravděpodobnosti, s jakými mohou být jednotlivé rarity vygenerovány
    /// </summary>
    private static Dictionary<Rarity, double> rarityProbabilities = new Dictionary<Rarity, double>
    {
        {Rarity.Common, 0.85 },
        {Rarity.Rare, 0.14 },
        {Rarity.Unique, 0.1 }
    };

    /// <summary>
    /// vygenerování náhodné kvality předmětu (pravděpodobnosti nejsou rozloženy stejnoměrně)
    /// </summary>
    /// <returns>náhodně vygenerovaná kvalita</returns>
    public static Quality RandomQuality()
    {
        double total = 0;
        foreach (double weight in qualityProbabilities.Values)
            total += weight;

        double random = (double)UnityEngine.Random.Range(0, 1);
        double sum = 0;
        foreach (Quality quality in Enum.GetValues(typeof(Quality)))
        {
            sum += qualityProbabilities[quality];
            if (random <= sum)
                return quality;
        }

        return Quality.Basic;
    }

    /// <summary>
    /// vygenerování náhodné rarity předmětu (pravděpodobnosti rarit nejsou rozloženy náhodně)
    /// </summary>
    /// <returns>nááhodně vygenerovaná rarita</returns>
    public static Rarity RandomRarity()
    {
        double total = 0;
        foreach (double weight in rarityProbabilities.Values)
            total += weight;

        double random = (double)UnityEngine.Random.Range(0f, 1f);
        double sum = 0;
        foreach (Rarity rarity in Enum.GetValues(typeof(Rarity)))
        {
            sum += rarityProbabilities[rarity];
            if (random <= sum)
                return rarity;
        }

        return Rarity.Common;
    }

    /// <summary>
    /// Náhodně zamíchá List.
    /// </summary>
    /// <typeparam name="T">Nějaký parametr listu.</typeparam>
    /// <param name="list">List který bude zamíchán.</param>
    /// <returns>zamíchaný list</returns>
    public static List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }
}


