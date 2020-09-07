using UnityEditor;
using UnityEngine;

// IngredientDrawer
[CustomPropertyDrawer(typeof(Weaknesses))]
public class WeaknessEditor: PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var anlRect = new Rect(position.x+ (position.width*2 / 15), position.y, position.width/5, position.height);
        var algRect = new Rect(position.x + position.width/ 3 + (position.width * 2 / 15), position.y, position.width/5, position.height);
        var numRect = new Rect(position.x + position.width*2 / 3 + (position.width * 2 / 15), position.y, position.width/5, position.height);

        var text1 = new Rect(position.x, position.y, position.width*2 / 15, position.height);
        var text2 = new Rect(position.x + position.width / 3 , position.y, position.width *2/ 15, position.height);
        var text3 = new Rect(position.x + position.width * 2 / 3 , position.y, position.width *2/ 15, position.height);

        GUIStyle right = new GUIStyle();
        right.alignment = TextAnchor.MiddleRight;
        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.LabelField(text1,"Anl ",right);
        EditorGUI.PropertyField(anlRect, property.FindPropertyRelative("AnlMult"), GUIContent.none);
        EditorGUI.LabelField(text2, "Alg ",right);
        EditorGUI.PropertyField(algRect, property.FindPropertyRelative("AlgMult"), GUIContent.none);
        EditorGUI.LabelField(text3, "Num ",right);
        EditorGUI.PropertyField(numRect, property.FindPropertyRelative("NumMult"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}