using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPattern : ScriptableObject
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
    /// <summary>
    /// sprite předmětu
    /// </summary>
    public Sprite sprite;
    /// <summary>
    /// komentar u itemu
    /// </summary>
    public string itemComment;
    /// <summary>
    /// Dostane tento item hrac na zacatku hry?
    /// </summary>
    public bool isStarting = false;

    public bool fixedRarity = false;
    public Rarity fixedRarityValue = Rarity.Common;

    public virtual ItemType Type() {
        return ItemType.none;
    }
}