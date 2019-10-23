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

    static public EnemyProperties[] Merge(EnemyBundle[] bundles,bool shuffle=true) {
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
        if (shuffle) {
            for (int i = 0; i < 500; i++)
            {
                int a = Random.Range(0,totalCount);
                int b = Random.Range(0,totalCount);
                EnemyProperties temp = array[a];
                array[a] = array[b];
                array[b] = temp;
            }
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

    public bool lootAfterFinish = true;

    public bool isSecret = false;

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

    public LevelGenerator.GeneratorPreset generatorPreset = LevelGenerator.GeneratorPreset.normal;

    public LevelGenerator.RoomConnectionPreset roomConnections =LevelGenerator.RoomConnectionPreset.addShortUnillAll;

    public ItemPattern[] loot;

    public SecretRoom[] secretRooms;

    Level(int difficulty, EnemyBundle[] enemies)
    {
        this.difficulty = difficulty;
        this.enemies = enemies;
    }
}

public enum SecretRoomType{ extraRandomItem , unlockLevel, extraItem };

[System.Serializable]
public class SecretRoom {
    public SecretRoomType type = SecretRoomType.extraRandomItem;
    public string unlockedLevel;
    public ItemPattern[] loot;
}