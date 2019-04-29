using System;
using System.IO;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public static class LocalizedUtil {

	// 换行占位符
	public const string NEWLINE_PLACEHOLDER = "$$$";

	public static string ReplaceNewLine(string str) {
		return str.Replace("\r\n", NEWLINE_PLACEHOLDER)
			.Replace("\r", NEWLINE_PLACEHOLDER)
			.Replace("\n", NEWLINE_PLACEHOLDER);
	}
	public static string RecoverNewLine(string str) {
		return str.Replace(NEWLINE_PLACEHOLDER, "\r\n");
	}



	public static Regex RegexChineseStr = new Regex(@"[\u4e00-\u9fa5]");


	public static bool ContainChinese(this string str) {
		return RegexChineseStr.IsMatch(str);
	}

	/// <summary>
	/// 将字符串本地化(扩展方法)
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	public static string ToLocalized(this string str) {
		return LocalizedDict.Localized(str);
	}
	
	public static string ToLocalized(this string str, params object[] p) {
		return LocalizedDict.Localized(str,p);
	}
	
	public static string t(this string str) {
		return LocalizedDict.Localized(str);
	}
	
	public static string t(this string str, params object[] p) {
		return LocalizedDict.Localized(str,p);
	}
}
