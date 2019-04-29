using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;


class CodeScanner {


	public static string[] IgnoreFileByKeyWord = new string[]
	{
		"/Editor/",//忽略Editor目录下的代码
		"GMConsole",
		"ModAnalytics",
		"GUI"
	};


	[MenuItem("XPlugin/本地化C#/提取 选中目录中 所有cs文件中 String 生成字典")]
	public static void AutoFindCode() {
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);

		// 重置字典数据
		_dictionaryStr = "";
		_txtKey.Clear();
		_txtValue.Clear();
		_valueRepeatCount = 0;
		_keyRepeatCount = 0;
		_txtPath = path.Replace("/", "_") + ".txt";

		FindAllCode(path);

		EditorUtility.DisplayProgressBar("本地化", "保存字典到Txt", 1);
		for (var i = 0; i < _txtKey.Count; i++) {
			var key = _txtKey[i];
			var value = _txtValue[i];
			value = LocalizedUtil.ReplaceNewLine(value);
			_dictionaryStr += (i + 1) + "\t" + key + "\t" + value + "\r\n";
		}
		EditorUtility.ClearProgressBar();
		File.WriteAllText(_txtPath, _dictionaryStr);
		// 输出结果
		Debug.Log("Find " + _txtKey.Count + " Keys.");
		if (_keyRepeatCount > 0 || _valueRepeatCount > 0) {
			Debug.Log("Value repeat : " + _valueRepeatCount + " , Key repeat ; " + _keyRepeatCount);
		}
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}


	/// <summary>
	/// 字典数据
	/// </summary>
	private static string _dictionaryStr;
	private static readonly List<string> _txtKey = new List<string>();
	private static readonly List<string> _txtValue = new List<string>();
	private static readonly List<string> _excelKey = new List<string>();
	private static readonly List<string> _excelValue = new List<string>();

	private static string _txtPath;

	/// 转义引号占位符
	/// </summary>
	private static string _quotationMark = Guid.NewGuid().ToString();

	private static int _valueRepeatCount;
	private static int _keyRepeatCount;

	#region Regex
	/// <summary>
	/// 本地化代码
	/// </summary>
	private static string localizeCode = "(LString.{0}).ToLocalized()";

	/// <summary>
	/// 中文正则
	/// </summary>
	private static Regex regexChineseStr = new Regex("\"[^\"]*[\u4e00-\u9fa5]+[^\"]*\"");
	/// <summary>
	/// 命名空间正则
	/// </summary>
	private static Regex regexNameSpace = new Regex(".*namespace.*");
	/// <summary>
	/// 类名正则
	/// </summary>
	private static Regex regexClass = new Regex(".*class.*");
	/// <summary>
	/// 函数正则
	/// </summary>
	private static Regex regexFunction = new Regex("[^\\.\\s=]+\\s+[^\\.\\s=]+\\([^!=]*\\)\\s*\\{?[^\\;]*\\s*$");
	#endregion


	/// <summary>
	/// 查找所有代码
	/// </summary>
	static void FindAllCode(string path) {
		var count = 0;
		// 获取所有object的路径
		var prefabFile = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
		// 循环遍历每一个路径，单独加载
		foreach (var filePath in prefabFile) {
			// 显示进度
			var fileName = Path.GetFileNameWithoutExtension(filePath);
			EditorUtility.DisplayProgressBar("本地化Code " + (count + 1) + "/" + prefabFile.Length, "当前：" + fileName, (float)(count + 1) / prefabFile.Length);
			// 替换路径中的反斜杠为正斜杠       
			var strTempPath = filePath.Replace(@"\", "/");
			// 截取我们需要的路径
			if (!CheckFileNeedLocalize(strTempPath)) {
				continue;
			}
			// 本地化处理
			LocalizeCode(strTempPath);
			count++;
		}
	}

	#region Code Localize
	/// <summary>
	/// 本地化代码文件
	/// </summary>
	/// <param name="codePath">文件路径</param>
	private static void LocalizeCode(string codePath) {
		var lines = new List<string>(File.ReadAllLines(codePath));
		// 初始化
		var spaceName = "";
		var className = "";
		var funcName = "";
		var funcIndex = 0;
		var classIndex = 0;
		// 逐行处理代码
		var gmCode = false;
		bool needResave = false;
		for (var i = 0; i < lines.Count; i++) {
			var line = lines[i];
			// 结束和忽略检查
			if (CheckGMStart(line)) {
				gmCode = true;
				continue;
			}
			if (gmCode && CheckGMEnd(line)) {
				gmCode = false;
			}
			if (gmCode) {
				continue;
			}
			if (CheckIgnore(line)) continue;
			// 查找命名空间、类名、函数名
			if (string.IsNullOrEmpty(spaceName)) {
				spaceName = FindNameSpace(line);
			}
			if (string.IsNullOrEmpty(className)) {
				className = FindClass(line);
			}
			var func = FindFunction(line);
			if (func != funcName && !string.IsNullOrEmpty(func)) {
				funcName = func;
				funcIndex = 0;
			}
			// 暂时替换转义引号以配合正则表达式
			line = line.Replace("\\\"", _quotationMark);
			// 匹配需要替换的字符串
			var match = regexChineseStr.Match(line);
			line = line.Replace(_quotationMark, "\\\"");
			while (match.Length > 0) {
				needResave = true;
				// 匹配到后构造Key和Value
				var key = CreateKey(spaceName, className, funcName, classIndex, funcIndex);
				var valueOrginal = match.Value.Replace(_quotationMark, "\\\"");
				// 截取双引号
				var value = valueOrginal.Substring(1, valueOrginal.Length - 2);
				// TXT Value 重复检查
				int index;
				var valueRepeat = false;
				if ((index = _txtValue.IndexOf(value)) > -1) {
					key = _txtKey[index];
					_valueRepeatCount++;
					valueRepeat = true;
				}
				// Excel Value 重复检查
				if ((index = _excelValue.IndexOf(value)) > -1) {
					key = _excelKey[index];
					_valueRepeatCount++;
					valueRepeat = true;
				}
				// TXT Key 重复检查
				if (_txtKey.IndexOf(key) > -1 && !valueRepeat) {
					_keyRepeatCount++;
					// 添加重复标记
					var keyTemp = key + "_" + _keyRepeatCount;
					while (_excelKey.IndexOf(keyTemp) > -1) {
						_keyRepeatCount++;
						keyTemp = key + "_" + _keyRepeatCount;
					}
					key = keyTemp;
				}
				// Excel Key 重复检查
				if (_excelKey.IndexOf(key) > -1 && !valueRepeat) {
					var num = 1;
					// 添加重复标记
					var keyTemp = key + "_" + num;
					while (_excelKey.IndexOf(keyTemp) > -1) {
						num++;
						keyTemp = key + "_" + num;
					}
					key = keyTemp;
				}
				// 替换代码
				var codeReplace = CreateCode(key);
				line = line.Replace(valueOrginal, codeReplace);
				// Debug.Log("Key : " + key + "\t Value : " + value + "\t Code : " + codeReplace);
				if (!valueRepeat) {
					// 添加到字典
					_txtKey.Add(key);
					_txtValue.Add(value);
					if (!string.IsNullOrEmpty(funcName)) funcIndex++;
					else classIndex++;
				}
				// 匹配下一个
				match = match.NextMatch();
			}
			lines[i] = line;
		}
		// 保存代码
		if (needResave) {
			File.WriteAllLines(codePath, lines.ToArray());
		}
	}
	#endregion


	#region File Check
	/// <summary>
	/// 检查文件是否需要做本地化处理
	/// </summary>
	/// <param name="assetPath">ASEET下的路径</param>
	private static bool CheckFileNeedLocalize(string assetPath) {
		foreach (var s in IgnoreFileByKeyWord) {
			if (assetPath.Contains(s)) {
				return false;
			}
		}
		return true;
	}
	#endregion

	#region Code & Key
	/// <summary>
	/// 创建键值
	/// </summary>
	/// <param name="spaceName">命名空间</param>
	/// <param name="className">类</param>
	/// <param name="funcName">函数</param>
	/// <param name="classIndex">类索引</param>
	/// <param name="funcIndex">函数索引</param>
	/// <returns></returns>
	private static string CreateKey(string spaceName, string className, string funcName, int classIndex, int funcIndex) {
		var result = "";
		if (!string.IsNullOrEmpty(spaceName)) {
			result += spaceName;
		}
		if (!string.IsNullOrEmpty(className)) {
			result += "_" + className;
		}
		if (!string.IsNullOrEmpty(funcName)) {
			result += "_" + funcName;
			if (funcIndex > 0) {
				result += "_" + funcIndex;
			}
		} else {
			if (classIndex > 0) {
				result += "_" + classIndex;
			}
		}
		if (result.Substring(0, 1) == "_") {
			result = result.Substring(1, result.Length - 1);
		}
		return result.ToUpper();
	}

	/// <summary>
	/// 根据键创建用于替换的代码
	/// </summary>
	/// <param name="key">键</param>
	/// <returns></returns>
	private static string CreateCode(string key) {
		var result = "";
		if (!string.IsNullOrEmpty(key)) {
			result = string.Format(localizeCode, key);
		}
		return result;
	}
	#endregion

	#region Code Check
	/// <summary>
	/// 检测该文件是否可以结束
	/// </summary>
	/// <param name="code">代码</param>
	/// <returns></returns>
	private static bool CheckGMStart(string code) {
		var str = code.Trim();
		// 遇到GM代码时，文件结束
		if (str.Length >= 13 && str.Substring(0, 13) == "#if CLIENT_GM") {
			return true;
		}
		return false;
	}

	/// <summary>
	/// 检测该文件是否可以结束
	/// </summary>
	/// <param name="code">代码</param>
	/// <returns></returns>
	private static bool CheckGMEnd(string code) {
		var str = code.Trim();
		// 遇到GM代码时，文件结束
		if (str.Length >= 6 && str.Substring(0, 6) == "#endif") {
			return true;
		}
		return false;
	}

	/// <summary>
	/// 检查代码是否需要忽略
	/// </summary>
	/// <param name="code"></param>
	/// <returns></returns>
	private static bool CheckIgnore(string code) {
		var str = code.Trim().ToLower();
		// 忽略空行
		if (str.Length < 1) {
			return true;
		}
		// 忽略宏定义
		if (str.Length >= 1 && str.Substring(0, 1) == "#") {
			return true;
		}
		// 忽略注释
		if (str.Length >= 2 && str.Substring(0, 2) == "//") {
			return true;
		}
		// 忽略引用
		if (str.Length >= 5 && str.Substring(0, 5) == "using") {
			return true;
		}
		// 忽略结尾
		if (str.Length >= 1 && str.Substring(0, 1) == "}") {
			return true;
		}
		// 忽略Log
		if (str.IndexOf("debug.", StringComparison.Ordinal) > -1 || str.IndexOf(".log", StringComparison.Ordinal) > -1) {
			return true;
		}
		// 忽略属性
		str = str.Trim().Replace(" ", "");
		var index1 = str.IndexOf("[", StringComparison.Ordinal);
		var index2 = str.IndexOf("(", StringComparison.Ordinal);
		var index3 = str.IndexOf(")]", StringComparison.Ordinal);
		if (index1 > -1 && index2 > index1 && index3 > index2 && index3 == str.Length - 2) {
			return true;
		}
		return false;
	}
	#endregion

	#region Code Match
	/// <summary>
	/// 查找命名空间
	/// </summary>
	/// <param name="code">代码</param>
	/// <returns>结果</returns>
	private static string FindNameSpace(string code) {
		var result = "";
		// 匹配命名空间
		var match = regexNameSpace.Match(code);
		if (match.Length == 0) {
			return result;
		}
		// 提取命名空间
		var str = code.Split(' ');
		if (str.Length > 1) {
			if (str[0].ToLower() == "namespace") {
				result = str[1];
			}
		}
		result = result.Replace("{", "");
		return result;
	}

	/// <summary>
	/// 查找类名
	/// </summary>
	/// <param name="code">代码</param>
	/// <returns>结果</returns>
	private static string FindClass(string code) {
		var result = "";
		// 匹配类名
		var match = regexClass.Match(code);
		if (match.Length == 0) {
			return result;
		}
		// 提取类名
		var classIndex = code.IndexOf("class", StringComparison.Ordinal);
		if (classIndex > -1) {
			var temp = code.Substring(classIndex, code.Length - classIndex - 1);
			var str = temp.Split(' ');
			result = str[1];
		}
		result = result.Replace("{", "");
		return result;
	}

	/// <summary>
	/// 查找函数名
	/// </summary>
	/// <param name="code">代码</param>
	/// <returns>结果</returns>
	private static string FindFunction(string code) {
		var result = "";
		// 匹配函数
		var match = regexFunction.Match(code);
		if (match.Length == 0) {
			return result;
		} else
			// 忽略定义
			if (code.IndexOf("new", StringComparison.Ordinal) > -1) {
			return result;
		}
		// 忽略()=>{}
		if (code.IndexOf("=>", StringComparison.Ordinal) > -1) {
			return result;
		}
		// 提取函数名
		var index = -1;
		index = code.IndexOf('(');
		if (index > -1) {
			var temp = code.Substring(0, index);
			var str = temp.Split(' ');
			result = str.Last();
		}
		// 忽略<>
		index = result.IndexOf('<');
		if (index > -1) {
			result = result.Substring(0, index);
		}
		return result;
	}
	#endregion
}
