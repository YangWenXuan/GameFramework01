using System;
using UnityEngine;
using UnityEditor;


/// <summary>
/// MeshRendererのsortingLayer/sortingOrder拡張
/// </summary>
[CustomEditor(typeof(MeshRenderer)), CanEditMultipleObjects]
public class MeshSortingOrderEditor : DecoratorEditor {

	private SortingOrderVars sortingOrderVars;

	public MeshSortingOrderEditor() : base("MeshRendererEditor") {
	}

	private void OnEnable() {
		sortingOrderVars = new SortingOrderVars();
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		sortingOrderVars.DrawInspector(serializedObject);
		this.serializedObject.ApplyModifiedProperties();
	}

}