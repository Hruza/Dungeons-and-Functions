using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// Trida, ktera obsahuje informace o enemy a ukazatel na jeho prefab
/// </summary>
[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy")]
public class EnemyProperties : ScriptableObject {

    public enum AIType {melee, ranged, spawning };
    /// <summary>
    /// Jmeno enemaka
    /// </summary>
    public new string name;
    public int orderID=1;
    public Sprite sprite;

    public int score = 1;

    public int baseHP;

    public Weaknesses weaknesses;

    public Damager.DamageType damageType;
    public AIType aiType;

    public int damage;


    private int level;
    public int Level {
        get
        {
            return level;
        }
        set
        {
            level = Mathf.Max(0, value);
        }
    }

    public GameObject EnemyGameObject;
}
