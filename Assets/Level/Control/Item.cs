using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Třída reprezentující předmět, který může hráč získat.
/// </summary>
public class Item : ScriptableObject
{
    /// <summary>
    /// výčtový typ určující typ předmětu
    /// </summary>
    public enum ItemType { MeeleWeapon, RangeWeapon, Armor}
    /// <summary>
    /// typ předmětu
    /// </summary>
    public ItemType Type { get; private set; }
    /// <summary>
    /// brnění, má smysl pouze pro brnění (a případně štíty, pokud budou)
    /// </summary>
    public int Armor { get; private set; }
    /// <summary>
    /// o kolik se zvýší brnění hráče (v absolutních číslech)
    /// </summary>
    public int BonusArmorAdditive { get; private set; }
    /// <summary>
    /// o kolik se zvýší brnění hráče (v procentech)
    /// </summary>
    public int BonusArmorMultiplicative { get; private set; }
    /// <summary>
    /// poškození předmětu, má smysl pouze u zbraní
    /// </summary>
    public int Damage { get; private set; }
    /// <summary>
    /// o kolik se zvýší celkové poškození hráče (aditivní)
    /// </summary>
    public int BonusDamageAdditive { get; private set; }
    /// <summary>
    /// o kolik se zvýší poškození hráče (v procentech)
    /// </summary>
    public int BonusDamageMultiplicative { get; private set; }
    /// <summary>
    /// o kolik se zvýší hráčova regenerace
    /// </summary>
    public int Regeneration { get; private set; }

    private void Start()
    {
        //nějaká základní inicializace
        Armor = 0;
        BonusArmorAdditive = 0;
        BonusArmorMultiplicative = 0;
        Damage = 0;
        BonusDamageAdditive = 0;
        BonusDamageMultiplicative = 0;
        Regeneration = 0;
    }
}
