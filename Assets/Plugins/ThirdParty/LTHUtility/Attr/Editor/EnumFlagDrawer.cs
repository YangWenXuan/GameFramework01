﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		Enum targetEnum = GetBaseProperty<Enum>(property);

		var propName = property.name;

		EditorGUI.BeginProperty(position, label, property);
		Enum enumNew = EditorGUI.EnumFlagsField(position, propName, targetEnum);
		property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
		EditorGUI.EndProperty();
	}

	static T GetBaseProperty<T>(SerializedProperty prop) {
		// Separate the steps it takes to get to this property
		string[] separatedPaths = prop.propertyPath.Split('.');

		// Go down to the root of this serialized property
		object reflectionTarget = prop.serializedObject.targetObject;
		// Walk down the path to get the target object
		foreach (var path in separatedPaths) {
			FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
			reflectionTarget = fieldInfo.GetValue(reflectionTarget);
		}
		return (T)reflectionTarget;
	}
}