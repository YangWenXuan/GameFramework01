
using System;
using System.Security.Cryptography;
using System.Text;

namespace XPlugin.Security {

	public static class AESUtil {

		public static byte[] Encrypt(byte[] data, string key, string iv) {
			byte[] bKey = Encoding.UTF8.GetBytes(key);
			byte[] ivArray = Encoding.UTF8.GetBytes(iv);

			RijndaelManaged rDel = new RijndaelManaged {
				Key = bKey,
				IV = ivArray,
				Mode = CipherMode.CBC,
				Padding = PaddingMode.Zeros
			};

			ICryptoTransform cTransform = rDel.CreateEncryptor();
			byte[] result = cTransform.TransformFinalBlock(data, 0, data.Length);

			return Encoding.UTF8.GetBytes(Convert.ToBase64String(result, 0, result.Length));
		}

		private static byte[] DecryptInternal(string data, string key, string iv) {
			byte[] toDecrypt = Convert.FromBase64String(data);
			byte[] keyArray = Encoding.UTF8.GetBytes(key);
			byte[] ivArray = Encoding.UTF8.GetBytes(iv);

			RijndaelManaged rDel = new RijndaelManaged {
				Key = keyArray,
				IV = ivArray,
				Mode = CipherMode.CBC,
				Padding = PaddingMode.Zeros
			};

			ICryptoTransform cTransform = rDel.CreateDecryptor();
			byte[] result = cTransform.TransformFinalBlock(toDecrypt, 0, toDecrypt.Length);
			//移除解码后的尾部的空字节
			int i = result.Length - 1;
			while (result[i] == 0) {
				--i;
			}
			byte[] trimed = new byte[i + 1];
			Array.Copy(result, trimed, i + 1);
			return trimed;
		}

		public static byte[] Decrypt(byte[] data, string key, string iv) {
			return DecryptInternal(Encoding.UTF8.GetString(data), key, iv);
		}

		public static string Encrypt(string data, string key, string iv) {
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			byte[] result = Encrypt(bytes, key, iv);
			return Encoding.UTF8.GetString(result);
		}

		public static string Decrypt(string data, string key, string iv) {
			byte[] result = DecryptInternal(data, key, iv);
			return Encoding.UTF8.GetString(result);
		}

	}
}