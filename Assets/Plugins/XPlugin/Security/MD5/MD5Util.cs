using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XPlugin.Security {

	public static class MD5Util {

		/// <summary>
		/// 获取文件md5
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static string GetMD5ForFile(string filePath) {
			FileStream file = new FileStream(filePath, System.IO.FileMode.Open);
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++) {
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}


		/// <summary>
		/// 从bytes获取md5
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string GetMD5(byte[] bytes) {
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(bytes);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++) {
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}

		/// <summary>
		/// 从字符串获取md5
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string GetMD5(string text) {
			return GetMD5(Encoding.UTF8.GetBytes(text));
		}

		public static bool EqualIgnoreCase(this string thiz, string other) {
			return thiz.Equals(other, StringComparison.OrdinalIgnoreCase);
		}

	}
}