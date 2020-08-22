using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.ProBuilder;

/// <summary>
/// všechny možné typy předmětů
/// </summary>
public enum ItemType { Armor, Weapon, none };
/// <summary>
/// všechny možné typy zbraní
/// </summary>
public enum WeaponType { Melee, Ranged };
/// <summary>
/// možné kvality předmětů
/// </summary>
public enum Quality { Basic, C };
/// <summary>
/// možné rarity zbraní
/// </summary>
public enum Rarity
{
    Common = 0,
    Rare = 1,
    Unique = 2,
    Legendary = 3
};

/// <summary>
/// Třída reprezentující předmět, který může hráč získat.
/// </summary>
public class Item
{
    public ItemPattern pattern;
    /// <summary>
    /// úroveň předmětu
    /// </summary>
    public int itemLevel{
        get {
            return pattern.level + (quality == Quality.C ?1 : 0);
        }
    }
    /// <summary>
    /// jméno předmětu
    /// </summary>
    public string itemName
    {
        get
        {
            return pattern.name + (quality==Quality.C?" +C":"");
        }
    }
    /// <summary>
    /// kvalita předmětu
    /// </summary>
    public Quality quality;
    /// <summary>
    /// rarita předmětu
    /// </summary>
    public Rarity rarity
    {
        get
        {
            return pattern.rarity;
        }
    }
    /// <summary>
    /// typ itemu
    /// </summary>
    public ItemType itemType;

    const int weaponPropability= 60;
    const int armorPropability= 40;

    /// <summary>
    /// sprite itemu
    /// </summary>
    public Sprite sprite
    {
        get
        {
            return pattern.sprite;
        }
    }

    /// <summary>
    /// komentar k itemu, ukaze se v tooltipu
    /// </summary>
    public string ItemComment {
        get {
            return pattern.itemComment;
        }
    }
    /// <summary>
    /// seznam všech bpnusových statů, které má předmět
    /// </summary>
    public Stat[] itemStats;


    /// <summary>
    /// bezparametrický kontruktor (nic neudělá)
    /// </summary>
    public Item()
    {
    }

    public Item(ItemPattern patt)
    {
        pattern = patt;
    }

    /// <summary>
    /// Delegát obsahující metodu sloužící pro vygenerování předmětu.
    /// </summary>
    /// <param name="item">základ předmětu</param>
    /// <returns>vygenerovaný předmět</returns>
    private delegate Item GeneratingMethods(Item item);


    public static Item Generate(int levelDifficulty, int score)
    {

        int rand = UnityEngine.Random.Range(1, 101);
        Item item;
        ItemPattern pattern = PickPattern(levelDifficulty,score);
        switch (pattern.Type())
        {
            case ItemType.Weapon:
                item = WeaponItem.Generate((WeaponPattern)pattern);
                break;
            case ItemType.Armor:
                item = ArmorItem.Generate((ArmorPattern)pattern);
                break;
            default:
                item = new Item();
                break;
        }
        return item;
    }


    static int lastDiff = -1;
    static int lastScore = -1;
    static List<ItemPattern> possiblePatts;
    static float[] props;
    static float sum;
    static ItemPattern PickPattern(int levelDifficulty, int score) {
        if (levelDifficulty != lastDiff || score != lastScore) {
            lastDiff = levelDifficulty;
            lastScore = score;
            possiblePatts = WeaponPattern.AllWeaponPatterns.FindAll(p => (p.level <= levelDifficulty && p.obtainableAsDrop)).ConvertAll(x =>(ItemPattern)x );
            possiblePatts.AddRange(ArmorPattern.AllArmorPatterns.FindAll(p => (p.level <= levelDifficulty && p.obtainableAsDrop)).ConvertAll(x => (ItemPattern)x ));
            props = new float[possiblePatts.Count];
            sum = 0;
            for (int i=0; i<possiblePatts.Count; i++)
            {
                props[i] = (float)possiblePatts[i].level / levelDifficulty;
                props[i] /= Mathf.Pow( 2,(int)possiblePatts[i].rarity );
                props[i] *= Distribution(possiblePatts[i].EvaluateScore(), score);
                sum += props[i];
            }
        }

        float rng = UnityEngine.Random.Range(0,sum);
        float partialSum = 0;
        for (int i = 0; i < possiblePatts.Count; i++)
        {
            partialSum += props[i];
            if (partialSum > rng) {
                return possiblePatts[i];
            }
        }
        Debug.LogError("Something went horribly wrong in item picking");
        return null;
    }

    public static float Distribution(float x,float center) {
        x =2.5f* (x - center) / center;
        return x<=0? Mathf.Exp(-2*x*x) : 
                     x>0.7? 0 :
                          Mathf.Exp(-10*x)  ;
    }

    public static Item Generate(ItemPattern pattern, bool noStats=false) {
        Item item = null;
        switch (pattern.Type())
        {
            case ItemType.Armor:
                item=ArmorItem.Generate( (ArmorPattern)pattern,noStats);
                break;
            case ItemType.Weapon:
                item=WeaponItem.Generate((WeaponPattern)pattern, noStats);
                break;
            default:
                break;
        }
        return item;
      
    }

    /// <summary>
    /// metoda, která generuje náhodné staty pro předmět
    /// </summary>
    public void GenerateStats()
    {
        int numberOfStats = (int)rarity;
        if (StatPattern.AllStatPatterns == null) {
            Debug.LogError("Stats are not initiated");
            return;
        }
        List<StatPattern> possibleStatPatterns = StatPattern.AllStatPatterns.Where<StatPattern>
                                                (s => (s.possibleItems.Contains(itemType) == true)).ToList();
        
        if (possibleStatPatterns.Count == 0)
        {
            Debug.Log("Neexistují vhodné staty pro předmět.");
            Debug.Log(this);
            return;
        }
        possibleStatPatterns = possibleStatPatterns.Shuffle();

        numberOfStats = Mathf.Min(numberOfStats, possibleStatPatterns.Count); //může se stát, že nebude dost statů
        itemStats = new Stat[numberOfStats];
        for (int i = 0; i < numberOfStats; i++)
        {
            Stat stat = new Stat();
            stat.name = possibleStatPatterns[i].name;
            stat.value = UnityEngine.Random.Range(possibleStatPatterns[i].lowerRange, possibleStatPatterns[i].upperRange + 1);
            stat.value += Mathf.RoundToInt(possibleStatPatterns[i].incrementPerLvl * (itemLevel - 1));
            itemStats[i] = stat;
        }
    }

    public int EvaluateScore() {
        return pattern.EvaluateScore() + (quality==Quality.C?ItemPattern.levelCoefficient:0);
    }

    void Upgrade() {
        if (quality == Quality.Basic)
        {
            quality = Quality.C;
        }
        else { 
            
        }
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


