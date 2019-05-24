using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public Vector3 eulerAngle;
	public bool timeScale = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
				transform.Rotate (eulerAngle* (timeScale ? Time.deltaTime : Time.unscaledDeltaTime));
	}
}
