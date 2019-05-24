using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(WatchAttr))]
public class WatchAttrDrawer : PropertyDrawer {


	// Here you must define the height of your property drawer. Called by Unity.
	public override float GetPropertyHeight(SerializedProperty prop,
											 GUIContent label) {

		WatchAttr w = (WatchAttr)attribute;
		string[] ws = w.ws;

		return EditorGUIUtility.singleLineHeight * (ws.Length + 1);
	}



	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		WatchAttr w = (WatchAttr)attribute;
		string[] ws = w.ws;
		object obj = property.serializedObject.targetObject;

		position.height = EditorGUIUtility.singleLineHeight;

		EditorGUI.BeginDisabledGroup(true);
		EditorGUI.LabelField(position, "监视值");
		position.y += position.height;

		foreach (var v in ws) {
			position.x = 0f;
			position.width = 100f;
			EditorGUI.LabelField(position, v);
			position.x += position.width;

			FieldInfo fieldInfo = GetField(property, v);
			Type fieldType = fieldInfo.FieldType;
			if (fieldType == typeof(int)) {
				EditorGUI.IntField(position, (int)(fieldInfo.GetValue(obj)));
			} else if (fieldType == typeof(string)) {
				EditorGUI.TextField(position, (string)(fieldInfo.GetValue(obj)));
			} else if (fieldType == typeof(float)) {
				EditorGUI.FloatField(position, (float)(fieldInfo.GetValue(obj)));
			} else if (fieldType == typeof(bool)) {
				EditorGUI.Toggle(position, (bool)(fieldInfo.GetValue(obj)));
			}
			
			else if (fieldType == typeof(UnityEngine.Object)) {
				EditorGUI.ObjectField(position, (UnityEngine.Object)(fieldInfo.GetValue(obj)),fieldType,true);
			}


			position.y += position.height;
		}

		EditorGUI.EndDisabledGroup();

	}

	public FieldInfo GetField(SerializedProperty property, string fieldName) {
		Type type = property.serializedObject.targetObject.GetType();
		FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
		return fieldInfo;
	}

}
