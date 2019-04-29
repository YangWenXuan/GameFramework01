using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using Object = UnityEngine.Object;

namespace XPlugin.UI {
    public class UIUpdateMenu {
        [MenuItem("XPlugin/UI/Update/记录选择的UI引用", false, 1)]
        static void Record() {
            RecordRecursive(Selection.activeGameObject);
            EditorUtility.SetDirty(Selection.activeGameObject);
            EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("XPlugin/UI/Update/还原选择的UI引用", false, 2)]
        static void Reassign() {
            ReassignRecursive(Selection.activeGameObject);
            EditorUtility.SetDirty(Selection.activeGameObject);
            EditorSceneManager.MarkSceneDirty(Selection.activeGameObject.scene);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("XPlugin/UI/Update/记录所有UI引用", false, 3)]
        static void RecordAll() {
            var files = Directory.GetFiles("Assets/_UI/Resources/UI", "*.prefab", SearchOption.AllDirectories);
            int i = 0;
            foreach (var file in files) {
                if (EditorUtility.DisplayCancelableProgressBar("记录所有UI引用", "", (float) i++ / files.Length)) {
                    break;
                }
                var g = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                RecordRecursive(g);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
        }

        public static void RecordRecursive(GameObject g) {
            ImageSpriteReassign.RecordAndRemoveReference(g, true);
            TextFontReassign.RecordAndRemoveReference(g, true);
            foreach (Transform child in g.transform) {
                RecordRecursive(child.gameObject);
            }
        }

        [MenuItem("XPlugin/UI/Update/还原所有UI引用", false, 4)]
        static void ReassignAll() {
            try {
                var files = Directory.GetFiles("Assets/_UI/Resources/UI", "*.prefab", SearchOption.AllDirectories);
                int i = 0;
                foreach (var file in files) {
                    if (EditorUtility.DisplayCancelableProgressBar("还原所有UI引用", "", (float) i++ / files.Length)) {
                        break;
                    }
                    var g = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                    ReassignRecursive(g);
                    EditorUtility.SetDirty(g);
                }
            } catch (Exception e) {
                Debug.LogException(e);
                throw;
            } finally {
                EditorUtility.ClearProgressBar();
                AssetDatabase.SaveAssets();
            }
        }


        public static void ReassignRecursive(GameObject g) {
            ImageSpriteReassign imageSpriteReassign = g.GetComponent<ImageSpriteReassign>();
            if (imageSpriteReassign != null) {
                imageSpriteReassign.ReAssign();
//                Object.DestroyImmediate(imageSpriteReassign, true);
            }
            TextFontReassign textFontReassign = g.GetComponent<TextFontReassign>();
            if (textFontReassign != null) {
                textFontReassign.ReAssign();
//                Object.DestroyImmediate(textFontReassign, true);
            }
            foreach (Transform v in g.transform) {
                ReassignRecursive(v.gameObject);
            }
        }
    }
}