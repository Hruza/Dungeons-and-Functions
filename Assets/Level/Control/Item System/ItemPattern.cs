using JetBrains.Annotations;
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


    public ItemPattern upgrade;

    public virtual ItemType Type() {
        return ItemType.none;
    }

    public const int levelCoefficient = 9;
    public const int rarityCoefficient = 5;
    public int EvaluateScore() {
        return ( levelCoefficient * level ) + ( rarityCoefficient * (int)rarity );
    }
}