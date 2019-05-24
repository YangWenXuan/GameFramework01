using UnityEngine;
using System.Collections;

namespace LTHUtility {

	public class CommonPlayerSetting : MonoBehaviour {
		public int targetFrameRate = 60;
		public bool neverSleep = true;

		// Use this for initialization
		void Awake() {
			Application.targetFrameRate = targetFrameRate;
			if (neverSleep) {
				Screen.sleepTimeout = SleepTimeout.NeverSleep;
			}

		}

	}
}
