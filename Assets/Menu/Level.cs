using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyBundle {
    public int count;
    public EnemyProperties enemyProperties;

    public EnemyProperties[] ToArray() {
        EnemyProperties[] array = new EnemyProperties[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = enemyProperties;
        }
        return array;
    }

    static public EnemyProperties[] Merge(EnemyBundle[] bundles) {
        int totalCount = 0;
        foreach (EnemyBundle bundle in bundles)
        {
            totalCount += bundle.count;
        }

        EnemyProperties[] array = new EnemyProperties[totalCount];
        int pos = 0;
        foreach (EnemyBundle bundle in bundles) {
            for (int i = 0; i < bundle.count; i++)
            {
                array[pos + i] = bundle.enemyProperties;
            }
            pos += bundle.count;
        }
        return array;
    }
}

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class Level : ScriptableObject
{
    private bool playable;
    public bool Playable { get { return playable; } set { playable = value; } }

    public string levelName;
    /// <summary>
    /// Jak daleko musí byt hrac, aby mohl hrat tento level
    /// </summary>
    public int progressID;

    /// <summary>
    /// Uroven enemies v levelu
    /// </summary>
    public int difficulty;

    /// <summary>
    /// Enemies v levelu
    /// </summary>
    public EnemyBundle[] enemies;
    /// <summary>
    /// Pocet mistnosi
    /// </summary>
    public int roomCount=1;
    Level(int difficulty, EnemyBundle[] enemies)
    {
        this.difficulty = difficulty;
        this.enemies = enemies;
    }
}