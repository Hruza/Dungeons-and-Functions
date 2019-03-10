using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
public enum Rarity
{
    Common = 0,
    Rare = 1,
    Unique = 2
};

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
    /// Konstanta určující, o kolik je vylepšen předmět.
    /// </summary>
    public const double qualityUpgrade = 1.07;
    /// <summary>
    /// Konstanta, která určuje o koik je lepší předmět vyšší rarity.
    /// </summary>
    public const double rarityUpgrade = 1.1;

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

    /// <summary>
    /// metoda, která generuje náhodné staty pro předmět
    /// </summary>
    public void GenerateStats()
    {
        int numberOfStats = UnityEngine.Random.Range(0, 2) +  (int)rarity;
        List<StatPattern> possibleStatPatterns = StatPattern.AllStatPatterns.Where<StatPattern>
                                                (s => (s.possibleItems.Contains(itemType) == true)).ToList();

        if (possibleStatPatterns.Count == 0)
        {
            Debug.Log("Neexistují vhodné staty pro předmět.", this);
            return;
        }
        possibleStatPatterns = possibleStatPatterns.Shuffle();

        numberOfStats = Math.Min(numberOfStats, possibleStatPatterns.Count); //může se stát, že nebude dost statů
        itemStats = new Stat[numberOfStats];
        for (int i = 0; i < numberOfStats; i++)
        {
            Stat stat = new Stat();
            stat.name = possibleStatPatterns[i].name;
            stat.value = UnityEngine.Random.Range(possibleStatPatterns[i].lowerRange, possibleStatPatterns[i].upperRange + 1);
            stat.value += possibleStatPatterns[i].incrementPerLvl * (itemLevel - 1);
            itemStats[i] = stat;
        }
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

    
}

/// <summary>
/// třída obsahující rozšiřující metody
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Metoda sloužící k najití dalšího prvku ve výčtovém typu.
    /// </summary>
    /// <typeparam name="T">výčtový typ (enum)</typeparam>
    /// <param name="src">prvek výčtového typu</param>
    /// <returns>Další prvek výčtového typu v pořadí</returns>
    public static T NextElement<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }

    /// <summary>
    /// Náhodně zamíchá List.
    /// </summary>
    /// <typeparam name="T">Nějaký parametr listu.</typeparam>
    /// <param name="list">List který bude zamíchán.</param>
    /// <returns>zamíchaný list</returns>
    public static List<T> Shuffle<T>(this List<T> list)
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


