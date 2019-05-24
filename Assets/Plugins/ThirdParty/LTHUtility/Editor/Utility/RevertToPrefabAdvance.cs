////
//// RevertToPrefab.cs
////
//// Author:
//// [longtianhong]
////
//// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
//
//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//
///// <summary>
///// Revert to prefab.
///// 这个脚本会开启一个窗口，可以讲场景中的物体全部替换为一个Prefab,且保持某些不变。
///// 在场景制作过程中非常实用
///// </summary>
//public class RevertToPrefabAdvance : EditorWindow {
//
//	public GameObject prefab;
//
//	public bool keepPosAndRot = true;
//	public bool keepScale = true;
//	public bool keepLightMap = true;
//
//
//
//	[MenuItem("LTH/Utility/替换物体")]
//	[MenuItem("GameObject/替换物体", false, 10)]
//	static void Init(MenuCommand menuCommand) {
//		EditorWindow.GetWindow<RevertToPrefabAdvance>();
//	}
//
//	void OnEnable() {
//		var o = Selection.activeGameObject;
//		if (o != null) {
//			var p = PrefabUtility.GetPrefabParent(o);
//			if (p != null) {
//				prefab = p as GameObject;
//			}
//		}
//	}
//
//	void OnGUI() {
//		if (GUILayout.Button("自动合为一组")) {
//			Transform[] selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
//			if (selection != null && selection.Length > 0 && selection[0] != null) {
//				Transform parent = new GameObject("zu_" + selection[0].name).transform;
//				Undo.RegisterCreatedObjectUndo(parent.gameObject, "");
//				parent.parent = selection[0].parent;
//				parent.localPosition = Vector3.zero;
//				parent.rotation = Quaternion.identity;
//				foreach (Transform t in selection) {
//					Undo.SetTransformParent(t, parent, "change parent");
//				}
//				EditorGUIUtility.PingObject(parent);
//			}
//		}
//		prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
//		keepPosAndRot = GUILayout.Toggle(keepPosAndRot, "保持位置和旋转");
//		keepScale = GUILayout.Toggle(keepScale, "保持缩放");
//		GUI.color = Color.yellow;
//		keepLightMap = GUILayout.Toggle(keepLightMap, new GUIContent("保持LightMap索引", "只有结构一致的物体才能保持索引"));
//		GUI.color = Color.white;
//		GUILayout.BeginHorizontal();
//		if (GUILayout.Button("自动prefab替换")) {
//			OnEnable();
//			DoRevert();
//		}
//		if (GUILayout.Button(new GUIContent("GO"))) {
//			DoRevert();
//		}
//		GUILayout.EndHorizontal();
//	}
//
//	void DoRevert() {
//		if (prefab != null) {
//			Transform[] selection = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
//			foreach (Transform t in selection) {
//				GameObject ins = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
//				Undo.RegisterCreatedObjectUndo(ins, "ins prefab");
//				ins.transform.parent = t.parent;
//				if (keepPosAndRot) {
//					ins.transform.localPosition = t.localPosition;
//					ins.transform.eulerAngles = t.eulerAngles;
//				}
//				if (keepScale) {
//					ins.transform.localScale = t.localScale;
//				}
//
//				if (keepLightMap) {
//					var insRenderer = ins.GetComponentsInChildren<Renderer>();
//					var originRenderer = t.GetComponentsInChildren<Renderer>();
//					for (int i = 0; i < insRenderer.Length; i++) {
//						if (i < originRenderer.Length) {
//							insRenderer[i].lightmapIndex = originRenderer[i].lightmapIndex;
//							insRenderer[i].lightmapScaleOffset = originRenderer[i].lightmapScaleOffset;
//						} else {
//							Debug.LogError("原物体与现物体Renderer不一致:" + t);
//						}
//					}
//				}
//
//				Undo.DestroyObjectImmediate(t.gameObject);
//			}
//
//		} else {
//			Debug.Log("no prefab choose");
//		}
//	}
//}
