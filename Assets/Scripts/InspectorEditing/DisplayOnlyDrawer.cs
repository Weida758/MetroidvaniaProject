
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Disables editing of a serialized field in the inspector. Use [field: DisplayOnly] for [field: SerializeField]
/// and [DisplayOnly] for [SerializeField]
/// </summary>
[CustomPropertyDrawer(typeof(DisplayOnlyAttribute))]
public class DisplayOnlyDrawer : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
    }
}
#endif
