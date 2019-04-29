using UnityEngine;
using System.Collections;

namespace XPlugin.UI {
    public class TextFontFix : MonoBehaviour {
        void Awake() {
            Font.textureRebuilt += OnTextureRebuilt;
        }

        void OnDestroy() {
            Font.textureRebuilt -= OnTextureRebuilt;
        }

        public void OnTextureRebuilt(Font font) {
            TextFontReassign[] fontReassigns = this.GetComponentsInChildren<TextFontReassign>();
            if (fontReassigns != null && fontReassigns.Length > 0) {
                foreach (var t in fontReassigns) {
                    t.UpdateFont(font);
                }
            }
        }
    }
}