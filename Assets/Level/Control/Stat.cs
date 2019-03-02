using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    /// <summary>
    /// jméno bonusového statu
    /// </summary>
    public string name;
    /// <summary>
    /// hodnota statu
    /// </summary>
    public int value;
}

public class PossibleStat
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
    public int incrementPerLvl;
    /// <summary>
    /// možné typy itemů, na kterých se stat může vygenerovat
    /// </summary>
    public List<ItemType> possibleItems;

    public static List<PossibleStat> AllPossibleStats = new List<PossibleStat>
    {
        new PossibleStat
        {
            name = "DamageAdditive",
            lowerRange = 1,
            upperRange = 5,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Weapon }
        },
        new PossibleStat
        {
            name = "DamageMultiplicative",
            lowerRange = 1,
            upperRange = 3,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Weapon }
        },
        new PossibleStat
        {
            name = "DamageAdditive",
            lowerRange = 1,
            upperRange = 5,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Armor }
        },
        new PossibleStat
        {
            name = "ArmorMultiplicative",
            lowerRange = 1,
            upperRange = 3,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Armor }
        },
        new PossibleStat
        {
            name = "Regeneration",
            lowerRange = 1,
            upperRange = 3,
            incrementPerLvl = 1,
            possibleItems = { ItemType.Armor, ItemType.Weapon }
        }
    };
}
