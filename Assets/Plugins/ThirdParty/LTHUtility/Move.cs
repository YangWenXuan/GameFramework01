using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	public Vector3 speed = new Vector3(0f, 0f, 10f);

	void Update() {
		transform.Translate(speed * Time.deltaTime);
	}

}
