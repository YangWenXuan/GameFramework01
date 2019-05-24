using UnityEngine;

namespace LTHUtility {
	public static class LayerUtility {

		public static void SetLayer(this GameObject g, LayerMask layer) {
			g.layer = GetLayerIndex(layer);
		}

		public static void SetLayerRecursion(this GameObject g, LayerMask layer) {
			g.layer = GetLayerIndex(layer);
			foreach (Transform child in g.transform) {
				SetLayerRecursion(child.gameObject, layer);
			}
		}

		public static void SetLayerRecursionByIndex(this GameObject g, int layer) {
			g.layer = layer;
			foreach (Transform child in g.transform) {
				SetLayerRecursionByIndex(child.gameObject, layer);
			}
		}

		public static int GetLayerIndex(this LayerMask layer) {
			int i = 0;
			while (layer.value >> i != 0x1) {
				i++;
				if (i > 32) {
					Debug.LogError("获取layermask的序号时出错");
					return -1;
				}
			}
			return i;
		}

		public static LayerMask GetLayerMaskFromIndex(int index) {
			var ret = new LayerMask { value = 1 << index };
			return ret;
		}

	}
}