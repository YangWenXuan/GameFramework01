using UnityEngine;
using UnityEditor;

namespace LTHUtility {

	[CustomPropertyDrawer(typeof(RangeFloat))]
	public class RangeFloatDrawer : PropertyDrawer {

		// Draw the property inside the given rect
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			// Calculate rects
			var type = new Rect(position.x, position.y, 50, position.height);
			var point = new Rect(position.x + 50, position.y, 50, position.height);

			// Draw fields - passs GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(type, property.FindPropertyRelative("min"), GUIContent.none);
			EditorGUI.PropertyField(point, property.FindPropertyRelative("max"), GUIContent.none);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}
