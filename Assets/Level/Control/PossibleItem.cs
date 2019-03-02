using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleItem : ScriptableObject
{
    /// <summary>
    /// jméno předmětu
    /// </summary>
    public string name;
    /// <summary>
    /// minimální možný item level, který může předmět mít
    /// </summary>
    public int lowerItemLevel;
    /// <summary>
    /// maximální možný item level, který může předmět mít
    /// </summary>
    public int upperItemLevel;
}

public class PossibleWeapon : PossibleItem
{
    /// <summary>
    /// dolní hranice minimálnoho poškození zbraně
    /// </summary>
    public int lowerMinDamage;
    /// <summary>
    /// horní hranice minimálnoho poškození zbraně
    /// </summary>
    public int upperMinDamage;
    /// <summary>
    /// dolní hranice maximálního poškození zbraně
    /// </summary>
    public int lowerMaxDamage;
    /// <summary>
    /// horní hranice maximálního poškození zbraně
    /// </summary>
    public int upperMaxDamage;
    /// <summary>
    /// rychlost útoku zbraně
    /// </summary>
    public int attackSpeed;

    /// <summary>
    /// databáze všech možných zbraní
    /// </summary>
    public static List<PossibleWeapon> AllPossibleWeapons = new List<PossibleWeapon>
        {
            new PossibleWeapon
            {
                name = "ItegralSword",
                lowerItemLevel = 1,
                upperItemLevel = 1000,
                lowerMinDamage = 1,
                upperMinDamage = 4,
                lowerMaxDamage = 5,
                upperMaxDamage = 10,
                attackSpeed = 1
            }
        };
}

public class PossibleArmor : PossibleItem
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
    /// databáze všech možných brnění
    /// </summary>
    public static List<PossibleArmor> AllPossibleArmors = new List<PossibleArmor>
    {
        new PossibleArmor
        {
            name = "BasicArmor",
            lowerItemLevel = 1,
            upperItemLevel = 1000,
            lowerArmor = 1,
            upperArmor = 5,
            movementSpeedReduction = 0
        }
    };
}