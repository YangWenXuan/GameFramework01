using UnityEngine;
using System.Collections;
using UnityEditor;

public class CleanPlayerPreferences : MonoBehaviour {

	[MenuItem("LTH/Utility/Clean PlayerPreferences")]
	static void DeleteAllData() {
		PlayerPrefs.DeleteAll();
	}
}
