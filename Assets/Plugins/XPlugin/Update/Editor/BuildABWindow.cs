using System;
using UnityEngine;
using UnityEditor;
using XPlugin.UI;
using Object = UnityEngine.Object;

namespace XPlugin.Update {
    public class BuildABWindow : EditorWindow {
        public Object[] ToBuildCompressed;
        public Object[] ToBuildNoCompressed;

        private SerializedObject serializedObject;
        private SerializedProperty _toBuildComProperty;
        private SerializedProperty _toBuildNoComProperty;

        [MenuItem("XPlugin/Update/显示构建Bundle窗口", false, 1)]
        static void ShowWindow() {
            GetWindow<BuildABWindow>().Show();
        }


        void OnEnable() {
            serializedObject = new SerializedObject(this);
            _toBuildComProperty = serializedObject.FindProperty("ToBuildCompressed");
            _toBuildNoComProperty = serializedObject.FindProperty("ToBuildNoCompressed");
        }


        private Vector2 scrollPos;

        void OnGUI() {
            serializedObject.Update();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.Label("将要构建成Bundle的物体放到列表中");
            EditorGUILayout.PropertyField(_toBuildComProperty, true);
            EditorGUILayout.PropertyField(_toBuildNoComProperty, true);
            GUILayout.EndScrollView();
            serializedObject.ApplyModifiedProperties();


            if (GUILayout.Button("构建")) {
                foreach (var o in ToBuildCompressed) {
                    beforeBuild(o);
                    bool result = BuildAssetBundle.SimpleBuild(o, true);
                    afterBuild(o);
                    if (!result) {
                        EditorUtility.DisplayDialog("error", "构建Bundle时出错:" + o, "ok");
                        return;
                    }
                }
                foreach (var o in ToBuildNoCompressed) {
                    beforeBuild(o);
                    bool result = BuildAssetBundle.SimpleBuild(o, false);
                    afterBuild(o);
                    if (!result) {
                        EditorUtility.DisplayDialog("error", "构建Bundle时出错:" + o, "ok");
                        return;
                    }
                }
            }
            if (GUILayout.Button("打开构建AB路径")) {
                EditorUtility.RevealInFinder(BuildAssetBundle.FullAbBuildOutPutDir);
            }
        }

        //TODO 应该改为attr回调形式，并且是针对要打包的资源执行record,reassign
        protected void beforeBuild(Object obj) {
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.Contains("_UI/Resources/UI/")&&obj is GameObject gameObject) {
                UIUpdateMenu.RecordRecursive(gameObject);
            }
        }

        protected void afterBuild(Object obj) {
            string path = AssetDatabase.GetAssetPath(obj);
            if (path.Contains("_UI/Resources/UI/")&&obj is GameObject gameObject) {
                UIUpdateMenu.ReassignRecursive(gameObject);
            }
        }
    }
}