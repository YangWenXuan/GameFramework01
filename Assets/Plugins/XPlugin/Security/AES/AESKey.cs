using UnityEngine;

namespace XPlugin.Security {

	[CreateAssetMenu]
	public class AESKey : ScriptableObject {

		public const string DEFAULT_NAME = "AESKey";

		public string key;
		public string iv;

		public static string DefaultKey {
			get {
				return Key(DEFAULT_NAME);
			}
		}

		public static string DefaultIV {
			get {
				return IV(DEFAULT_NAME);
			}
		}


		public static string Key(string keyName) {
			return Resources.Load<AESKey>(keyName).key;

		}

		public static string IV(string keyName) {
			return Resources.Load<AESKey>(keyName).iv;
		}

	}
}
