using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(RawImagePot), true)]
[CanEditMultipleObjects]
public class RawPotSpriteEditor : RawImageEditor {
 
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
		RawImagePot circle = target as RawImagePot;
		circle.OriginSize = EditorGUILayout.Vector2Field("OriginSize", circle.OriginSize);
		if (GUILayout.Button("Set Origin Size")) {
			var rect = circle.rectTransform.rect;
			circle.SetOriginSize();
		}

	}
 
}