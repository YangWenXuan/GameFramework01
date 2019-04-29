using UnityEngine;
using XPlugin.Security;


namespace XPlugin.Security {

	public static class PlayerPrefsAES {

		public static void SetString(string key, string value) {
			PlayerPrefs.SetString(MD5Util.GetMD5(key), AESUtil.Encrypt(value, AESKey.DefaultKey, AESKey.DefaultIV));
		}

		public static void SetInt(string key, int value) {
			SetString(key, value.ToString());
		}

		public static void SetLong(string key, long value) {
			SetString(key, value.ToString());
		}

		public static void SetFloat(string key, float value) {
			SetString(key, value.ToString());
		}

		public static void SetDouble(string key, double value) {
			SetString(key, value.ToString());
		}

		public static void SetBool(string key, bool value) {
			SetString(key, value.ToString());
		}


		public static string GetString(string key, string opt = default(string)) {

			var ret = PlayerPrefs.GetString(MD5Util.GetMD5(key));
			if (string.IsNullOrEmpty(ret)) {
				return opt;
			} else {
				return AESUtil.Decrypt(ret, AESKey.DefaultKey, AESKey.DefaultIV);
			}
		}

		public static int GetInt(string key, int opt = default(int)) {
			string s = GetString(key);
			int ret;
			if (int.TryParse(s, out ret)) {
				return ret;
			}
			return opt;
		}

		public static long GetLong(string key, long opt = default(long)) {
			string s = GetString(key);
			long ret;
			if (long.TryParse(s, out ret)) {
				return ret;
			}
			return opt;
		}
		public static float GetFloat(string key, float opt = default(float)) {
			string s = GetString(key);
			float ret;
			if (float.TryParse(s, out ret)) {
				return ret;
			}
			return opt;
		}

		public static double GetDouble(string key, double opt = default(int)) {
			string s = GetString(key);
			double ret;
			if (double.TryParse(s, out ret)) {
				return ret;
			}
			return opt;

		}

		public static bool GetBool(string key, bool opt = default(bool)) {
			string s = GetString(key);
			bool ret;
			if (bool.TryParse(s, out ret)) {
				return ret;
			}
			return opt;
		}
	}
}
