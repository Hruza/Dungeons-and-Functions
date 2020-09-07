using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Projectile))]
[CanEditMultipleObjects]
public class ProjectileEditor : Editor
{
    const int labelWidth = 150;


    SerializedProperty detectionDistance;
    SerializedProperty detectionAngle;
    SerializedProperty turningSpeed;
    SerializedProperty timeToStart;
    SerializedProperty returnOnCollision;

    SerializedProperty explosionDamageMultiplicator;
    SerializedProperty explosionRadius;
    SerializedProperty explosionDamageDistribution;
    SerializedProperty explosionDamageEnemies;
    SerializedProperty explosionDamagePlayer;
    SerializedProperty explosionDamageDestroyables;

    SerializedProperty inflictSlowness;
    SerializedProperty slownessModifier;
    SerializedProperty slownessTime;

    void OnEnable()
    {
        detectionDistance = serializedObject.FindProperty("detectionDistance") ;
        detectionAngle = serializedObject.FindProperty("detectionAngle");
        turningSpeed = serializedObject.FindProperty("turningSpeed");
        timeToStart = serializedObject.FindProperty("timeToStart");
        returnOnCollision = serializedObject.FindProperty("returnOnCollision");

        explosionDamageMultiplicator =serializedObject.FindProperty("explosionDamageMultiplicator");
        explosionRadius=serializedObject.FindProperty("explosionRadius");
        explosionDamageDistribution=serializedObject.FindProperty("explosionDamageDistribution");
        explosionDamageEnemies=serializedObject.FindProperty("explosionDamageEnemies");
        explosionDamagePlayer=serializedObject.FindProperty("explosionDamagePlayer");
        explosionDamageDestroyables=serializedObject.FindProperty("explosionDamageDestroyables");

        slownessModifier = serializedObject.FindProperty("slownessModifier");
        slownessTime = serializedObject.FindProperty("slownessTime");

    }

    bool showHoming = false;
    bool showSlowness = false;
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        serializedObject.Update();
        Projectile tgt = (Projectile)target;
        
        if (tgt.explosion)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(explosionDamageMultiplicator, new GUIContent("Explosion damage multiplicator"));

            EditorGUILayout.PropertyField(explosionRadius, new GUIContent("Explosion radius"));

            EditorGUILayout.PropertyField(explosionDamageDistribution, new GUIContent("Explosion damage distribution"));

            EditorGUILayout.PropertyField(explosionDamageEnemies, new GUIContent("ExplosionDamageEnemies"));

            EditorGUILayout.PropertyField(explosionDamagePlayer, new GUIContent("ExplosionDamagePlayer"));

            EditorGUILayout.PropertyField(explosionDamageDestroyables, new GUIContent("ExplosionDamageDestroyables"));

            EditorGUI.indentLevel--;
        }

        if (tgt.inflictSlowness)
        {
            showSlowness = EditorGUILayout.Foldout(showSlowness, new GUIContent("Slowness"), EditorStyles.boldFont);
            if (showSlowness)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(slownessTime, new GUIContent("Slowness Time"));

                EditorGUILayout.PropertyField(slownessModifier, new GUIContent("Slowness modifier"));

                EditorGUI.indentLevel--;
            }
        }

        if (tgt.projectileType==Projectile.ProjectileType.homing)
        {

            showHoming=EditorGUILayout.Foldout(showHoming, new GUIContent("Homing"), EditorStyles.boldFont);

            if (showHoming)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(detectionDistance, new GUIContent("Detection distance"));
                EditorGUILayout.PropertyField(detectionAngle, new GUIContent("Detection angle"));
                EditorGUILayout.PropertyField(turningSpeed, new GUIContent("Turning speed"));
                
                EditorGUI.indentLevel--;

            }
        }
        if (tgt.projectileType == Projectile.ProjectileType.boomerang)
        {

            showHoming = EditorGUILayout.Foldout(showHoming, new GUIContent("Booemrang"), EditorStyles.boldFont);

            if (showHoming)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(turningSpeed, new GUIContent("Turning speed"));

                EditorGUILayout.PropertyField(timeToStart, new GUIContent("Time to return"));
                EditorGUILayout.PropertyField(returnOnCollision, new GUIContent("Return on collision"));

                EditorGUI.indentLevel--;

            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
