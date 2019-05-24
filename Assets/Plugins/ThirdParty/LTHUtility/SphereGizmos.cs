using UnityEngine;

namespace LTHUtility {
	public class SphereGizmos : MonoBehaviour {

#if UNITY_EDITOR
		public float Size = 1f;
		public bool Wire = true;
		public Color Color = Color.red;
		void OnDrawGizmos() {
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.color = this.Color;
			if (this.Wire) {
				Gizmos.DrawWireSphere(Vector3.zero, this.Size);
			} else {
				Gizmos.DrawWireSphere(Vector3.zero, this.Size);
			}
		}

#endif

	}
}