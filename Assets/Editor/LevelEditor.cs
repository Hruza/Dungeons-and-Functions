using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Level lvl = (Level)target;

        int score = 0;
        score += lvl.roomCount * LevelResults.roomClearedScore;
        foreach (EnemyBundle bundle in lvl.enemies)
        {
            score += bundle.count * bundle.enemyProperties.score;
        }
        if (lvl.bossRoom != null) {
            foreach (EnemyBundle bundle in lvl.bossEnemies)
            {
                score += bundle.count * bundle.enemyProperties.score;
            }
        }
        GUILayout.Label("Maximum score: "+score.ToString());
    }
}
