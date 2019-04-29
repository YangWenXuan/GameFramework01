using System;
using UnityEngine;
using UnityEngine.UI;
using XPlugin.Localization;
using XPlugin.Update;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace XPlugin.UI {
    [DisallowMultipleComponent]
    public class TextFontReassign : MonoBehaviour {

        public static Func<string, Font> onFontReq;
        
        public string fontPath;
        private Text text;

        void Awake() {
            ReAssign();
        }

        public void UpdateFont(Font font) {
            if (text != null && text.font == font) {
                text.FontTextureChanged();
            }
        }

        [ContextMenu("Resign")]
        public void ReAssign() {
            if (text == null) {
                text = GetComponent<Text>();
            }
            if (text != null) {
                if (onFontReq != null) {
                    text.font = onFontReq(this.fontPath);
                } else {
                    if (Application.isPlaying) {
                        text.font = LResources.Load<Font>(this.fontPath);
                    } else {
                        text.font = Resources.Load<Font>(this.fontPath);
                    }
                }
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Record")]
        void RecordAndRemoveReference() {
            RecordAndRemoveReference(this.gameObject, false);
            EditorUtility.SetDirty(gameObject);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }


        public static void RecordAndRemoveReference(GameObject g, bool removeReference) {
            var text = g.GetComponent<Text>();
            if (text == null) {
                return;
            }
            Font font = text.font;
            if (font == null || font.name == "" || font.name.Equals("Arial", StringComparison.OrdinalIgnoreCase)) {
                Debug.LogWarning("No Font:" + AnimationUtility.CalculateTransformPath(g.transform, null));
                return;
            }
            var reassign = g.GetOrAddComponent<TextFontReassign>();

            var path = AssetDatabase.GetAssetPath(font);
            path = UIUpdateUtil.GetResourcePath(path);
            reassign.fontPath = path;
            if (removeReference) {
                text.font = null;
            }
        }
#endif
    }
}