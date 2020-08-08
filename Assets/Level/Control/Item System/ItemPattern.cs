using System;
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
    /// muze byt jako odmena za level
    /// </summary>
    public bool obtainableAsDrop=true;
    /// <summary>
    /// minimální obtížnost, při které se 
    /// </summary>
    public int level;
    /// <summary>
    /// rarita predmetu
    /// </summary>
    public Rarity rarity;
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

    public virtual ItemType Type() {
        return ItemType.none;
    }

    const int levelCoefficient = 5;
    const int rarityCoefficient = 10;
    public int EvaluateScore() {
        return ( levelCoefficient * level ) + ( rarityCoefficient * (int)rarity );
    }
}