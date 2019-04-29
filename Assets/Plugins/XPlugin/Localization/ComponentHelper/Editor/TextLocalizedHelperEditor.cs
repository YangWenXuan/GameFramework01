using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEngine.UI;

namespace XPlugin.Localization {

	[CustomEditor(typeof(TextLocalizedHelper)), CanEditMultipleObjects]
	public class TextLocalizedHelperEditor : Editor {

		public List<string> keys;

		private SerializedProperty keyProperty;

		private LanguageEnum language = LanguageEnum.zh;//这是我们的默认语言

		string description = "";

		private TextLocalizedHelper helper;

		private string _error;

		void OnEnable() {
			helper = (TextLocalizedHelper)target;
			keys = new List<string>();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Type keysType=null;
			foreach (var ass in assemblies) {
				if (ass.GetName().Name == "Assembly-CSharp") {
					keysType = ass.GetType("LString");
					break;
				}
			}
			if (keysType == null) {
				_error = "没有找到自动生成的常量Key类 LString";
				Debug.LogError(_error);
				return;
			}
			var fields = keysType.GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (var fieldInfo in fields) {
				keys.Add(fieldInfo.Name);
			}

			keyProperty = serializedObject.FindProperty("Key");
		}

		public override void OnInspectorGUI() {
			if (!string.IsNullOrEmpty(_error)) {
				EditorGUILayout.HelpBox(_error, MessageType.Error);
				return;
			}


			serializedObject.Update();
			bool find = keys.Contains(keyProperty.stringValue);
			if (find) {
				GUI.color = Color.green;
			} else {
				GUI.color = Color.red;
				description = "没有找到";
			}
			EditorGUILayout.PropertyField(keyProperty);
			GUI.color = Color.white;

			language = (LanguageEnum)EditorGUILayout.EnumPopup("选择语言:", language);
			if (GUILayout.Button("显示翻译")) {


				Localization.Language = language;
				LocalizedDict.UnLoad();

				var keysType = typeof(TextLocalizedHelper).Assembly.GetType("LString");
				if (keysType == null) {
					Debug.LogError("没有找到自动生成的常量Key类 LString");
				} else {
					//找到类中所有static函数，执行一遍
					var methods = keysType.GetMethods(BindingFlags.Static | BindingFlags.Public);
					foreach (var methodInfo in methods) {
						methodInfo.Invoke(null, null);
					}
				}
				description = keyProperty.stringValue.ToLocalized();
			}
			EditorGUILayout.TextArea(description);

			if (helper.GetComponent<Text>() == null) {
				EditorGUILayout.HelpBox("这个组件需要和UnityEngine.UI.Text一起使用", MessageType.Error);
			}

			serializedObject.ApplyModifiedProperties();

		}
	}
}