using UnityEngine;
using System.Collections;


namespace XPlugin.Security {

	[CreateAssetMenu]
	public class DESKey : ScriptableObject {

		public const string DEFAULT_NAME = "DESKey";

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
			return Resources.Load<DESKey>(keyName).key;

		}

		public static string IV(string keyName) {
			return Resources.Load<DESKey>(keyName).iv;
		}

	}
}
