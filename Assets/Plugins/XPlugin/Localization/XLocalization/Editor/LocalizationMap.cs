using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XLocalization {

	// 本地化中使用的键值对Map
	class LocalizationMap {

		private List<string> keyArray = new List<string>();
		private List<string> valueArray = new List<string>();

		// 插入键值对返回键值
		// 当Value重复时 忽略本次插入 返回已存在的键值
		// 当Key重复值时 自动为Key添加后缀
		public string Insert(string key, string value) {
			// Value 重复
			int valueIndex = valueArray.IndexOf(value);
			if (valueIndex > -1) {
				return keyArray[valueIndex];
			}
			// Key 重复
			int keyIndex = valueArray.IndexOf(key);
			if (keyIndex > -1) {
				for (int count = 1; count < int.MaxValue; ++count) {
					if (valueArray.IndexOf(key + count) > -1) {
						key = key + count;
					}
				}
			}
			// 添加
			keyArray.Add(key);
			valueArray.Add(value);
			return key;
		}

		// 获取长度
		public int Length() {
			return keyArray.Count;
		}

		// 索引Key
		public string IndexKey(int index) {
			return keyArray[index];
		}

		// 索引Value
		public string IndexValue(int index) {
			return valueArray[index];
		}

		// 测试Key是否存在
		public bool ExistKey(string key) {
			return keyArray.Contains(key);
		}

		// 将LocalizationMap保存到TXT
		public void WriteToTxt(string txtPath) {
			string txtFileString = "";
			for (int i = 0; i < Length(); ++i) {
				var key = IndexKey(i);
				if (string.IsNullOrEmpty(key)) {
					continue;
				}
				var value = IndexValue(i);
				txtFileString += (i + 1) + "\t" + key + "\t" + value + "\r\n";
			}
			File.WriteAllText(txtPath, txtFileString);
			TextEditor te = new TextEditor();
			te.text = txtFileString;
			te.SelectAll();
			te.Copy();
			Debug.Log("已经复制到剪切板");
		}

	}
}
