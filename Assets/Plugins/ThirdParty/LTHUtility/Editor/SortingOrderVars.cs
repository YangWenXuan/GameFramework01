using UnityEditor;
using UnityEngine;

public class SortingOrderVars {
	public SortingOrderVars() {
		string[] sortingLayerNames = GetSortingLayerNames();
		layerIDContents = new GUIContent[sortingLayerNames.Length];
		for (int i = 0; i < sortingLayerNames.Length; ++i)
			layerIDContents[i] = new GUIContent(sortingLayerNames[i]);

		sortingLayerIds = GetSortingLayerUniqueIDs();
	}

	public int[] sortingLayerIds { get; set; }
	public GUIContent[] layerIDContents { get; set; }

	/// <summary>
	/// ソートレイヤー名取得
	/// </summary>
	public string[] GetSortingLayerNames() {
		System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
		System.Reflection.PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, null);
	}

	/// <summary>
	/// ソートレイヤーID取得
	/// </summary>
	public int[] GetSortingLayerUniqueIDs() {
		System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
		System.Reflection.PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
		return (int[])sortingLayerUniqueIDsProperty.GetValue(null, null);
	}

	public void DrawInspector(SerializedObject serializedObject) {
		SerializedProperty propSortingLayerID = serializedObject.FindProperty("m_SortingLayerID");
		SerializedProperty propSortingOrder = serializedObject.FindProperty("m_SortingOrder");

		EditorGUILayout.IntPopup(propSortingLayerID, layerIDContents, sortingLayerIds);
		EditorGUILayout.PropertyField(propSortingOrder);
	}
}
