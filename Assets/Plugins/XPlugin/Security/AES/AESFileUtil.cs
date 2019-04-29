using System.IO;

namespace XPlugin.Security {

	public static class AESFileUtil {
		public static string ReadAllText(string path, string key, string iv) {
			var text = File.ReadAllText(path);
			return AESUtil.Decrypt(text, key, iv);
		}

		public static void WriteAllText(string path, string text, string key, string iv) {
			var bs = AESUtil.Encrypt(text, key, iv);
			File.WriteAllText(path, bs);
		}

		public static byte[] ReadAllBytes(string path, string key, string iv) {
			var text = File.ReadAllBytes(path);
			return AESUtil.Decrypt(text, key, iv);
		}

		public static void WriteAllBytes(string path, byte[] bytes, string key, string iv) {
			var bs = AESUtil.Encrypt(bytes, key, iv);
			File.WriteAllBytes(path, bs);
		}

	}

}
