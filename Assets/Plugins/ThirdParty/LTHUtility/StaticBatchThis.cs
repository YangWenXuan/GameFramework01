using UnityEngine;
using System.Collections;

namespace LTHUtility {
	public class StaticBatchThis : MonoBehaviour {

		void Start() {
			StaticBatchingUtility.Combine(gameObject);
		}

	}
}
