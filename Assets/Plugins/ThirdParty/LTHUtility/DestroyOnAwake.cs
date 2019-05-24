using UnityEngine;
using System.Collections;


namespace LTHUtility {

	public class DestroyOnAwake : MonoBehaviour {
		void Awake() {
			DestroyImmediate(gameObject);
		}
	}
}
