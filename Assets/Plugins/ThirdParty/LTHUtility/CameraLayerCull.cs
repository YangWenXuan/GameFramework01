using NaughtyAttributes;
using UnityEngine;
//using HeavyDutyInspector;

namespace LTHUtility {

	public class CameraLayerCull : Singleton<CameraLayerCull> {

		[System.Serializable]
		public struct LayerCullData {
			//[Layer]
			public int Layer;
			public int Distance;
		}
		[ReorderableList]
		public LayerCullData[] LayerCull;

		/// <summary>
		/// 偏移(不同性能的设备可以设置不同的偏移)
		/// </summary>
		public static float Bias=1;

		void Start() {
			float[] distances = new float[32];
			for (int i = 0; i < this.LayerCull.Length; i++) {
				distances[this.LayerCull[i].Layer] = this.LayerCull[i].Distance*Bias;
			}
			GetComponent<Camera>().layerCullDistances = distances;
		}
	}
}