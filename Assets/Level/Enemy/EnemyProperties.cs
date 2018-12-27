using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trida, ktera obsahuje informace o enemy a ukazatel na jeho prefab
/// </summary>
[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy")]
public class EnemyProperties : ScriptableObject {
   
    /// <summary>
    /// Jmeno enemaka
    /// </summary>
    public new string name;

    
    public int level {
        get
        {
            return level;
        }
        set
        {
            level = Mathf.Max(1, level);
        }
    }

    public GameObject EnemyGameObject;
}
