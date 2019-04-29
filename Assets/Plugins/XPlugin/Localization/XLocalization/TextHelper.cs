//
// 用于 Unity5 UGUI 版本
//
#if UNITY_5

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using XPlugin.Localization;

namespace XLocalization {

	public class TextHelper : MonoBehaviour {

		public string FontPath;
		public string Key;
		public string OriginStringValue;//用于恢复

		[ContextMenu("调试-恢复文字")]
		void DebugRecover() {
			Text text = this.gameObject.GetComponent<Text>();
			if (null == text) {
				return;
			}
			if (!string.IsNullOrEmpty(FontPath)) {
				text.font = Resources.Load<Font>(FontPath);
			}
			if (string.IsNullOrEmpty(text.text)) {
				text.text = OriginStringValue;
			}
		}

		[ContextMenu("调试-清空引用")]
		void DebugClean() {
			Text text = this.gameObject.GetComponent<Text>();
			if (null == text) {
				return;
			}
			text.font = null;
			text.text = "";
		}

		void Awake() {
			Text text = this.gameObject.GetComponent<Text>();
			if (null == text) {
				return;
			}
			// font
			if (!string.IsNullOrEmpty(FontPath)) {
				text.font = LResources.Load<Font>(FontPath);
			}
			// text
			if (string.IsNullOrEmpty(text.text) && !string.IsNullOrEmpty(Key)) {
				text.text = Key.ToLocalized();
			}
		}
	}

}

#endif