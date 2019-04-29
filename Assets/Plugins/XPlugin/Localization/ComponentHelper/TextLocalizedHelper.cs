// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace XPlugin.Localization {

	/// <summary>
	/// 这个组件在start的时候自动根据key赋予UI.Text组件本地化后的字符串
	/// </summary>
	[RequireComponent(typeof(Text))]
	[DisallowMultipleComponent]
	public class TextLocalizedHelper : MonoBehaviour {

		public string Key;

		void Start() {
			var text = GetComponent<Text>();
			if (text != null) {
				text.text = Key.ToLocalized();
			} else {
				Debug.LogError("没有找到Text组件");
			}
		}
	}
}