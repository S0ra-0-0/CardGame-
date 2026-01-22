#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condAttr = (ConditionalHideAttribute)attribute;
        SerializedProperty sourceProperty = property.serializedObject.FindProperty(condAttr.conditionalSourceField);

        if (sourceProperty == null)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        bool enabled = condAttr.useEnum
            ? sourceProperty.enumValueIndex == condAttr.enumValue
            : sourceProperty.boolValue;

        if (enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condAttr = (ConditionalHideAttribute)attribute;
        SerializedProperty sourceProperty = property.serializedObject.FindProperty(condAttr.conditionalSourceField);

        if (sourceProperty == null)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        bool enabled = condAttr.useEnum
            ? sourceProperty.enumValueIndex == condAttr.enumValue
            : sourceProperty.boolValue;

        return enabled ? EditorGUI.GetPropertyHeight(property, label) : -EditorGUIUtility.standardVerticalSpacing;
    }
}
#endif


public class ConditionalHideAttribute : PropertyAttribute
{
    public string conditionalSourceField;
    public int enumValue;
    public bool useEnum;

    // For boolean fields
    public ConditionalHideAttribute(string conditionalSourceField)
    {
        this.conditionalSourceField = conditionalSourceField;
        this.useEnum = false;
    }

    // For enum fields
    public ConditionalHideAttribute(string conditionalSourceField, int enumValue)
    {
        this.conditionalSourceField = conditionalSourceField;
        this.enumValue = enumValue;
        this.useEnum = true;
    }
}


