using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ArmorPattern", menuName = "Armor Pattern")]
public class ArmorPattern : ItemPattern
{
    /// <summary>
    /// minimální možné brnění
    /// </summary>
    public int lowerArmor;
    /// <summary>
    /// maximální možné brnění
    /// </summary>
    public int upperArmor;
    /// <summary>
    /// o kolik je zpomalen hráč, když má toto brnění
    /// </summary>
    public int movementSpeedReduction;
    /// <summary>
    /// kolik armoru se prida za kazdy level
    /// </summary>
    public int armorIncrementPerLevel;
    /// <summary>
    /// databáze všech možných brnění
    /// </summary>
    public static List<ArmorPattern> AllArmorPatterns;
    /*new List<ArmorPattern>
{
    new ArmorPattern
    {
        name = "BasicArmor",
        lowerItemLevel = 1,
        upperItemLevel = 1000,
        lowerArmor = 1,
        upperArmor = 
,
        movementSpeedReduction = 0
    }
};*/
}
