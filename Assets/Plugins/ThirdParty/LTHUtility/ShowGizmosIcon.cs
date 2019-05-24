using UnityEngine;
using System.Collections;

public class ShowGizmosIcon : MonoBehaviour {

#if UNITY_EDITOR
	public string iconName;

	void OnDrawGizmos() {
		Gizmos.DrawIcon(transform.position, iconName);
	}

#endif


}
