using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections;

public class DisableFileTypeOpen : MonoBehaviour {

	private static string[] shieldFile = new string[]{
			"fbx",
			"obj",
			"mat",
		};

	[OnOpenAsset]
	private static bool CancelOpenModel(int instanceID, int line) {
		Object obj = EditorUtility.InstanceIDToObject(instanceID);
		string path = AssetDatabase.GetAssetPath(obj);

		foreach (var s in shieldFile) {
			if (path.EndsWith(s) || path.EndsWith(s.ToUpper()) || path.EndsWith(s.ToLower())) {
				Debug.Log("屏蔽打开" + s);
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
}
