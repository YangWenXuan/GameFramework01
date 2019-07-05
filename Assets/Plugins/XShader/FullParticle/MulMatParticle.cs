using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class MulMatParticle : MonoBehaviour {
	[ReorderableList]
	public Material[] mats;

	[ContextMenu("OnValidate")]
	void OnValidate() {
		GetComponent<ParticleSystemRenderer>().materials = mats;
	}

}
