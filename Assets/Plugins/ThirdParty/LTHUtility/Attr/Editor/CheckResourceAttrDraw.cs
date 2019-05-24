using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CheckResourceAttr))]
public class CheckResourceAttrDraw : PropertyDrawer {

	// Provide easy access to the RegexAttribute for reading information from it.
	CheckResourceAttr checkResourceAttr { get { return ((CheckResourceAttr)attribute); } }

	// Here you must define the height of your property drawer. Called by Unity.
	public override float GetPropertyHeight(SerializedProperty prop,
											 GUIContent label) {
		return base.GetPropertyHeight(prop, label);
	}

	// Here you can define the GUI for your property drawer. Called by Unity.
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {

		switch (Event.current.type) {
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!position.Contains(Event.current.mousePosition)) {
					return;
				}

				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (Event.current.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag();

					if (DragAndDrop.objectReferences.Length == 1) {
						prop.stringValue = DragAndDrop.objectReferences[0].name;
					}
				}
				break;
		}

		Color originColor = GUI.color;
		bool findObject = false;
		GameObject g = null;
		if (string.IsNullOrEmpty(prop.stringValue)) {
			GUI.color = Color.yellow;
		} else {
			g = Resources.Load<GameObject>(checkResourceAttr.path + prop.stringValue);
			if (g == null) {
				GUI.color = Color.red;
			} else {
				findObject = true;
				GUI.color = Color.green;
			}
		}
		position.width -= 20f;
		EditorGUI.PropertyField(position, prop, label);
		if (findObject) {
			position.x += position.width;
			position.width = 20f;
			if (GUI.Button(position, "●")) {
				EditorGUIUtility.PingObject(g);
			}
		}

		GUI.color = originColor;

	}


}