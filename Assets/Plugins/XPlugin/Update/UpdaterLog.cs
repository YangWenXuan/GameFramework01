#define UPDATE_LOG

using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


namespace XPlugin.Update {

	public class UpdaterLog {

		[Conditional("UPDATE_LOG")]
		public static void Log(object obj) {
			Debug.Log("[UPDATER_LOG] " + obj);
		}

		[Conditional("UPDATE_LOG")]
		public static void LogWaring(object obj) {
			Debug.LogWarning("[UPDATER_LOG] " + obj);
		}

		[Conditional("UPDATE_LOG")]
		public static void LogError(object obj) {
			Debug.LogError("[UPDATER_LOG] " + obj);
		}

		[Conditional("UPDATE_LOG")]
		public static void LogException(Exception e) {
			Debug.LogException(e);
		}


	}
}
