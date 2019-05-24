using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TrailRenderer)), CanEditMultipleObjects]
public class TrailSortingOrderEditor : Editor {
	private SortingOrderVars sortingOrderVars;

//	public TrailSortingOrderEditor() : base("TrailRendererEditor") {
//	}

	/// <summary>
	/// 選択時の初期化処理
	/// </summary>
	private void OnEnable() {
		sortingOrderVars = new SortingOrderVars();

	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		sortingOrderVars.DrawInspector(serializedObject);
		this.serializedObject.ApplyModifiedProperties();
	}


}