using UnityEngine;
using System.Collections;

namespace LTHUtility {
	public class ShowFPS : MonoBehaviour {

		public int fontSize = 16;

		private float updateInterval = 0.1f;
		private float lastInterval;
		private int frames = 0;
		private float fps;

		private GUIStyle style;
		void Start() {
			style = new GUIStyle();
		}

		void OnGUI() {
			float rate = Screen.height / 640f;
			style.fontSize = (int)(fontSize * rate);
			style.normal.textColor = fps < 30f ? Color.red : Color.green;
			Rect rect = new Rect(100f * rate, (float)Screen.height - 100f, 100f * rate, 20f * rate);
			GUI.Label(rect, "fps " + fps.ToString("f2"), style);
		}

		void Update() {
			++frames;
			float timeNow = Time.realtimeSinceStartup;
			if (timeNow > lastInterval + updateInterval) {
				fps = frames / (timeNow - lastInterval);
				frames = 0;
				lastInterval = timeNow;
			}
		}

	}
}
