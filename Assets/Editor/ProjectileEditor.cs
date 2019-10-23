using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
    const int labelWidth = 150;
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        Projectile tgt = (Projectile)target;

        if (tgt.explosion)
        {
            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("ExplosionDamageMultiplicator");
            tgt.explosionDamageMultiplicator = EditorGUILayout.FloatField(tgt.explosionDamageMultiplicator);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Explosion radius");
            tgt.explosionRadius = EditorGUILayout.FloatField(tgt.explosionRadius);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Explosion damage distribution");
            tgt.explosionDamageDistribution = EditorGUILayout.CurveField(tgt.explosionDamageDistribution);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("ExplosionDamageEnemies");
            tgt.explosionDamageEnemies = EditorGUILayout.Toggle(tgt.explosionDamageEnemies);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("ExplosionDamagePlayer");
            tgt.explosionDamagePlayer = EditorGUILayout.Toggle(tgt.explosionDamagePlayer);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("ExplosionDamageEnemies");
            tgt.explosionDamageDestroyables = EditorGUILayout.Toggle(tgt.explosionDamageDestroyables);
            GUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
        }
    }
}
