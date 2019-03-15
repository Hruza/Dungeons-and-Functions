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
}