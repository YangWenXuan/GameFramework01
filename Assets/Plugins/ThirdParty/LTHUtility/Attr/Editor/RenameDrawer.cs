using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RenameAttr))]
public class RenameDraw : PropertyDrawer {

	// Provide easy access to the RegexAttribute for reading information from it.
	RenameAttr renameAttr { get { return ((RenameAttr)attribute); } }

	// Here you must define the height of your property drawer. Called by Unity.
	public override float GetPropertyHeight(SerializedProperty prop,
											 GUIContent label) {
		return base.GetPropertyHeight(prop, label);
	}

	// Here you can define the GUI for your property drawer. Called by Unity.
	public override void OnGUI(Rect position,SerializedProperty prop,GUIContent label) {
		Color originColor = GUI.color;
		GUI.color = renameAttr.color;
		if (!string.IsNullOrEmpty(renameAttr.showName)) {
			label.text = renameAttr.showName;
		}
		EditorGUI.PropertyField(position, prop, label);
		GUI.color = originColor;
	}


}