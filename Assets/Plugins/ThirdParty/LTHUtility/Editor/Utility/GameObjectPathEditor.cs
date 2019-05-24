using UnityEngine;
using System.Collections;
using UnityEditor;

public class GameObjectPathEditor : MonoBehaviour {

	[MenuItem("LTH/Utility/Print Transform Path")]
	private static void CopyGameObjectPath() {
		UnityEngine.Object obj = Selection.activeObject;
		if (obj == null)
		{
			Debug.LogError("No GameObject");
			return;
		}
		string result = AssetDatabase.GetAssetPath(obj);
		// 如果不是资源则在场景中查找
		if (string.IsNullOrEmpty(result))
		{
			result= AnimationUtility.CalculateTransformPath(Selection.activeTransform, null);
		}
		ClipBoard.Copy(result);
		Debug.Log(string.Format("<color=green>[Path]</color>:{0} - {1}", obj.name, result));
	}

	/// <summary>
	/// 剪切板
	/// </summary>
	public class ClipBoard {
		/// <summary>
		/// 将信息复制到剪切板当中
		/// </summary>
		public static void Copy(string format, params object[] args) {
			string result = string.Format(format, args);
			TextEditor editor = new TextEditor();
			editor.text =result;
			editor.OnFocus();
			editor.Copy();
		}
	}
}
