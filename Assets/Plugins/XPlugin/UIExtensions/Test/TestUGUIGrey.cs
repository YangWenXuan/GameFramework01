using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestUGUIGrey : MonoBehaviour {

	[ContextMenu("toggle")]
	void Toggle() {
		GetComponent<Graphic>().SetGreyMaterail(!GetComponent<Graphic>().IsGrey());
	}

	void OnGUI() {
		if (GUILayout.Button("Toggle")) {
			Toggle();
		}
	}
}
