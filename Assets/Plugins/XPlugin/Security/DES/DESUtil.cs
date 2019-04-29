using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XPlugin.Security {
	public static class DESUtil {

		public static string DefaultDecrypt(string text) {
			return Decrypt(text, DESKey.DefaultKey, DESKey.DefaultIV);
		}

		public static string Decrypt(string text, string key, string iv) {
			using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider()) {
				provider.Key = Encoding.ASCII.GetBytes(key);
				provider.IV = Encoding.ASCII.GetBytes(iv);
				MemoryStream stream = new MemoryStream();
				using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write)) {
					byte[] buffer = Convert.FromBase64String(text);
					stream2.Write(buffer, 0, buffer.Length);
					stream2.FlushFinalBlock();
					stream2.Close();
				}
				string str = Encoding.UTF8.GetString(stream.ToArray());
				stream.Close();
				return str;
			}
		}

		public static byte[] DefaultDecrypt(byte[] bytes) {
			return Decrypt(bytes, DESKey.DefaultKey, DESKey.DefaultIV);
		}

		public static byte[] Decrypt(byte[] bytes, string key, string iv) {
			using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider()) {
				provider.Key = Encoding.ASCII.GetBytes(key);
				provider.IV = Encoding.ASCII.GetBytes(iv);
				MemoryStream stream = new MemoryStream();
				using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write)) {
					stream2.Write(bytes, 0, bytes.Length);
					stream2.FlushFinalBlock();
					stream2.Close();
				}
				byte[] convertByte = new byte[stream.ToArray().Length];
				Array.Copy(stream.ToArray(), convertByte, stream.ToArray().Length);
				stream.Close();
				return convertByte;
			}
		}

		public static string DefaultEncrypt(string text) {
			return Encrypt(text, DESKey.DefaultKey, DESKey.DefaultIV);
		}


		public static string Encrypt(string text, string key, string iv) {
			using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider()) {
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				provider.Key = Encoding.ASCII.GetBytes(key);
				provider.IV = Encoding.ASCII.GetBytes(iv);
				MemoryStream stream = new MemoryStream();
				using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write)) {
					stream2.Write(bytes, 0, bytes.Length);
					stream2.FlushFinalBlock();
					stream2.Close();
				}
				string str = Convert.ToBase64String(stream.ToArray());
				stream.Close();
				return str;
			}
		}

		public static byte[] DefaultEncrypt(byte[] bytes) {
			return Encrypt(bytes, DESKey.DefaultKey, DESKey.DefaultIV);
		}

		public static byte[] Encrypt(byte[] bytes, string key, string iv) {
			using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider()) {
				provider.Key = Encoding.ASCII.GetBytes(key);
				provider.IV = Encoding.ASCII.GetBytes(iv);
				MemoryStream stream = new MemoryStream();
				using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write)) {
					stream2.Write(bytes, 0, bytes.Length);
					stream2.FlushFinalBlock();
					stream2.Close();
				}
				byte[] convertByte = new byte[stream.ToArray().Length];
				Array.Copy(stream.ToArray(), convertByte, stream.ToArray().Length);
				stream.Close();
				return convertByte;
			}
		}

	}
}