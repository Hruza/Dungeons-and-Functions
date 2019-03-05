using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Trida, ktera obsahuje informace o zbrani a ukazatel na prefab
/// </summary>
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class WeaponProperty : Item {
   
    /// <summary>
    /// Jmeno zbrane
    /// </summary>
    public new string name;

    public Sprite sprite;

    public int level;

    public GameObject weaponGameObject;
}