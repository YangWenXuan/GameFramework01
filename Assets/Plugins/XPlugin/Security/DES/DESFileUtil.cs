using UnityEngine;
using System.Collections;
using System.IO;

namespace XPlugin.Security {
	public static class DESFileUtil {
		
		public static string DefaultRead(string text) {
			return ReadAllText(text, DESKey.DefaultKey, DESKey.DefaultIV);
		}

		public static void DefaultWrite(string path, string text) {
			WriteAllText(path, text, DESKey.DefaultKey, DESKey.DefaultIV);
		}

		public static void DefaultEncrypt(string path, string target) {
			WriteAllText(target, File.ReadAllText(path), DESKey.DefaultKey, DESKey.DefaultIV);
		}

		public static void DefaultDecrypt(string path, string target) {
			File.WriteAllText(target, ReadAllText(path, DESKey.DefaultKey, DESKey.DefaultIV));
		}

		public static string ReadAllText(string path, string key, string iv) {
			var text = File.ReadAllText(path);
			return DESUtil.Decrypt(text, key, iv);
		}

		public static void WriteAllText(string path, string text, string key, string iv) {
			var bs = DESUtil.Encrypt(text, key, iv);
			File.WriteAllText(path, bs);
		}

		public static byte[] ReadAllBytes(string path, string key, string iv) {
			var text = File.ReadAllBytes(path);
			return DESUtil.Decrypt(text, key, iv);
		}

		public static void WriteAllBytes(string path, byte[] bytes, string key, string iv) {
			var bs = DESUtil.Encrypt(bytes, key, iv);
			File.WriteAllBytes(path, bs);
		}


	}
}
