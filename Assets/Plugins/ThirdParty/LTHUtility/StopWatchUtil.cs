using System.Collections.Generic;
using System.Diagnostics;

namespace LTHUtility {
	public class StopWatchUtil {
		private static Dictionary<string, Stopwatch> watchs = new Dictionary<string, Stopwatch>();

		public static void Start(string name) {
			Stopwatch watch = new Stopwatch();
			watch.Start();
			watchs.Add(name, watch);
		}

		public static void Stop(string name) {
			var watch = watchs[name];
			UnityEngine.Debug.Log(name + " : " + watch.ElapsedMilliseconds);
			watch.Stop();
			watchs.Remove(name);
		}

	}
}