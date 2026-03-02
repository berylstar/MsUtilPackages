using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(InspectorReadonlyAttribute))]
public class InspectorReadonlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //// 기존 값 복구
        //bool prevValue = GUI.enabled;

        //GUI.enabled = false;
        //EditorGUI.PropertyField(position, property, label, true);
        //GUI.enabled = prevValue;

        EditorGUI.BeginDisabledGroup(true);

        EditorGUI.PropertyField(position, property, label, true);

        EditorGUI.EndDisabledGroup();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif