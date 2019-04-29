//
// 用于 Unity5 UGUI 版本
//
#if UNITY_5

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace XLocalization {

	public class TextScanner : MonoBehaviour {


		[MenuItem("XPlugin/本地化Text/标记 选中文件夹中Prefab的Text")]
		static void MarkPrefab() {
			MarkTextInPrefab(AssetDatabase.GetAssetPath(Selection.activeObject));
		}

		[MenuItem("XPlugin/本地化Text/还原 选中文件夹中Prefab的Text")]
		static void UnmarkPrefab() {
			UnmarkTextInPrefab(AssetDatabase.GetAssetPath(Selection.activeObject));
		}


		private static void MarkTextInPrefab(string path) {
			string txtPath =path.Replace("/", "_") + ".txt";
			LocalizationMap locMap = new LocalizationMap();

			string[] prefabArray = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
			for (int i = 0; i < prefabArray.Length; i++) {
				GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabArray[i], typeof(GameObject)) as GameObject;
				List<GameObjectInfo> infoArray = ListAllGameObjectWithText(prefab);
				foreach (GameObjectInfo info in infoArray) {
					GameObject gameObject = info.gameObject;
					TextHelper textHelper = MarkText(gameObject, info.key);
					if (null != textHelper && !string.IsNullOrEmpty(info.key)) {
						string newKey = locMap.Insert(textHelper.Key, textHelper.OriginStringValue);
						if (newKey != textHelper.Key) {
							textHelper.Key = newKey;//value重复
						}
					}
				}
				EditorUtility.DisplayCancelableProgressBar("标记Text", "", (float)i / prefabArray.Length);
			}
			EditorUtility.DisplayCancelableProgressBar("写入" + txtPath, "", 100);
			locMap.WriteToTxt(txtPath);
			EditorUtility.ClearProgressBar();
			AssetDatabase.SaveAssets();
		}

		private static void UnmarkTextInPrefab(string path) {
			string[] prefabArray = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);

			for (int i = 0; i < prefabArray.Length; i++) {
				GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabArray[i], typeof(GameObject)) as GameObject;
				List<GameObjectInfo> infoArray = ListAllGameObjectWithText(prefab);
				foreach (GameObjectInfo info in infoArray) {
					UnmarkText(info.gameObject);
				}
				EditorUtility.DisplayCancelableProgressBar("还原Text", "", (float)i / prefabArray.Length);
			}
			EditorUtility.ClearProgressBar();
			AssetDatabase.SaveAssets();
		}


		public static TextHelper MarkText(GameObject gameObject, string key) {
			if (null != gameObject.GetComponent<TextHelper>()) {
				Debug.LogError("貌似这个物体已经标记过了" + key);
				return null;
			}

			Text text = gameObject.GetComponent<Text>();
			if (null == text) {
				Debug.LogError("没有在这个物体上找到Text " + key);
				return null;
			}

			if (null == text.font) {
				Debug.LogError("这个物体的Font为空,最好检查一下" + key);
				return null;
			}
			TextHelper textHelper = gameObject.AddComponent<TextHelper>();
			textHelper.FontPath = LocalizedUtil.GetResourcePath(text.font);
			if (textHelper.FontPath != null) {//避开使用了非Resource下的字体
				text.font = null;
			}

			if (!string.IsNullOrEmpty(text.text) && text.text.ContainChinese() && !string.IsNullOrEmpty(key)) {
				textHelper.Key = key;
				textHelper.OriginStringValue = LocalizedUtil.ReplaceNewLine(text.text);
				textHelper.OriginStringValue = textHelper.OriginStringValue.Replace("\t", "    ");
				text.text = null;
			} else {
				//				Debug.Log("没有文字或者文字不包含中文，跳过这个： " + key);
			}
			//			Debug.Log("成功标记 -  " + textHelper.FontPath + "  -  " + gameObject.name);
			return textHelper;
		}

		// 删除GameObject上的TextHelper组件
		public static void UnmarkText(GameObject gameObject) {
			TextHelper textHelper = gameObject.GetComponent<TextHelper>();
			if (null == textHelper) {
				//				Debug.Log("这个物体没有被标记过" + gameObject);
				return;
			}

			Text text = gameObject.GetComponent<Text>();
			if (null == text) {
				Debug.LogError("这个物体上没有Text");
				return;
			}

			Font font = Resources.Load<Font>(textHelper.FontPath);
			if (null != font) {
				text.font = font;
			} else {
				Debug.LogError("没有找到字体资源：：：" + textHelper.FontPath);
			}

			text.text = textHelper.OriginStringValue;

			DestroyImmediate(textHelper, true);
			Debug.Log("成功恢复 -  " + textHelper.FontPath + "  -  " + gameObject.name);
		}


		private class GameObjectInfo {
			public GameObject gameObject;
			public string key;
		}

		/// <summary>
		/// 递归列出全部含Text组件的GameObject
		/// </summary>
		/// <param name="rootObject"></param>
		/// <returns></returns>
		private static List<GameObjectInfo> ListAllGameObjectWithText(GameObject rootObject) {
			List<GameObjectInfo> infoArray = new List<GameObjectInfo>();
			TravelGameObjectWithText(rootObject, infoArray);
			return infoArray;
		}

		private static int i = 0;

		private static void TravelGameObjectWithText(GameObject go, List<GameObjectInfo> infoArray) {
			// find Text in self
			if (go.GetComponent<Text>() != null) {
				GameObjectInfo info = new GameObjectInfo();
				info.gameObject = go;
				info.key = AnimationUtility.CalculateTransformPath(go.transform, null)
					.Replace(" ", "_")
					.Replace("/", "_")
					.Replace("(", "_")
					.Replace(")", "_");//取路径作为key
				Debug.Log(++i + " Key " + info.key + "Length : " + info.key.Length);
				infoArray.Add(info);
			}
			// travel children
			foreach (Transform child in go.transform) {
				TravelGameObjectWithText(child.gameObject, infoArray);
			}
		}
	}
}

#endif