using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class Level : ScriptableObject
{
    private bool playable;
    public bool Playable { get { return playable; } set { playable = value; } }
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
    public EnemyProperties[] enemies;
    /// <summary>
    /// Pocet mistnosi
    /// </summary>
    public int roomCount=1;
    Level(int difficulty, EnemyProperties[] enemies)
    {
        this.difficulty = difficulty;
        this.enemies = enemies;
    }
}