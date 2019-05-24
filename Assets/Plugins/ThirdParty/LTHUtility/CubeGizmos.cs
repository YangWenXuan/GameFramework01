using UnityEngine;
using System.Collections;

namespace LTHUtility {

	public class CubeGizmos : MonoBehaviour {


#if UNITY_EDITOR
		public Vector3 Size = new Vector3(0.2f, 0.2f, 0.4f);
		public bool Wire = false;
		public Color Color = Color.red;
		void OnDrawGizmos() {
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.color = this.Color;
			if (this.Wire) {
				Gizmos.DrawWireCube(Vector3.zero, this.Size);
			} else {
				Gizmos.DrawCube(Vector3.zero, this.Size);
			}
		}

#endif
	}
}
