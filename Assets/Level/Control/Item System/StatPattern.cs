using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "StatPattern", menuName = "Stat Pattern")]
public class StatPattern : ScriptableObject
{
    /// <summary>
    /// jméno typu bonusového statu
    /// </summary>
    public string name;
    /// <summary>
    /// dolní hranice ozsahu, v kterém se může stat vygenerovat na levelu 1
    /// </summary>
    public int lowerRange;
    /// <summary>
    /// horní hranice rozsahu, ve kterém se může stat vygenerovat na itemu levelu 1
    /// </summary>
    public int upperRange;
    /// <summary>
    /// o kolik se zvýší hodnota statu za každou úroveň předmětu nad 1
    /// </summary>
    public float incrementPerLvl;
    /// <summary>
    /// možné typy itemů, na kterých se stat může vygenerovat
    /// </summary>
    public List<ItemType> possibleItems;

    public static List<StatPattern> AllStatPatterns;
        /*new List<StatPattern>
    {
        new StatPattern
        {
            name = "DamageAdditive",
            lowerRange = 1,
            upperRange = 5,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Weapon }
        },
        new StatPattern
        {
            name = "DamageMultiplicative",
            lowerRange = 1,
            upperRange = 3,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Weapon }
        },
        new StatPattern
        {
            name = "ArmorAdditive",
            lowerRange = 1,
            upperRange = 5,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Armor }
        },
        new StatPattern
        {
            name = "ArmorMultiplicative",
            lowerRange = 1,
            upperRange = 3,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Armor }
        },
        new StatPattern
        {
            name = "Regeneration",
            lowerRange = 1,
            upperRange = 3,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Armor, ItemType.Weapon }
        }
    };*/
}

