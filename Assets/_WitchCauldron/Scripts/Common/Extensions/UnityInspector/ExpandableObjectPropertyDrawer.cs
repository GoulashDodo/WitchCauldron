using UnityEditor;
using UnityEngine;

namespace Common.Extensions.UnityInspector
{
    [CustomPropertyDrawer(typeof(ExpandableAttribute))]
    public class ExpandableObjectPropertyDrawer : PropertyDrawer
    {
        private Editor editor = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (property.objectReferenceValue == null) return;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                Rect contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                if (editor == null)
                {
                    editor = Editor.CreateEditor(property.objectReferenceValue);
                }

                if (editor != null)
                {
                    editor.OnInspectorGUI();
                }
                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label, true);
            if (property.isExpanded && property.objectReferenceValue != null)
            {
                // This is a rough estimate; a more accurate height would require iterating through the SO's properties.
                // For simplicity, we'll just add some extra height.
                height += EditorGUIUtility.singleLineHeight * 5; 
            }
            return height;
        }
    }
}