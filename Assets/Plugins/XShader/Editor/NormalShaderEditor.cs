using System;
using UnityEngine;
using UnityEditor;

public class NormalShaderEditor : ShaderGUI {

	public enum BlendMode {
		Opaque,
		Cutout,
		Transparent,
	}
	public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));

	private MaterialProperty blendMode;
	MaterialEditor m_MaterialEditor;
	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {
		Material material = materialEditor.target as Material;
		blendMode = FindProperty("_Mode", props);
		m_MaterialEditor = materialEditor;
		EditorGUI.BeginChangeCheck();
		{
			EditorGUI.showMixedValue = blendMode.hasMixedValue;
			var mode = (BlendMode)blendMode.floatValue;

			EditorGUI.BeginChangeCheck();
			mode = (BlendMode)EditorGUILayout.Popup("Rendering Type", (int)mode, blendNames);
			if (EditorGUI.EndChangeCheck()) {
				m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
				blendMode.floatValue = (float)mode;
			}

			EditorGUI.showMixedValue = false;
		}
		if (EditorGUI.EndChangeCheck()) {
			SetupMaterialWithBlendMode(material, (BlendMode)this.blendMode.floatValue);
		}
		base.OnGUI(materialEditor, props);
	}

	public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode) {
		switch (blendMode) {
			case BlendMode.Opaque:
				material.SetOverrideTag("RenderType", "");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = -1;
				break;
			case BlendMode.Cutout:
				material.SetOverrideTag("RenderType", "TransparentCutout");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.EnableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 2450;
				break;
			case BlendMode.Transparent:
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
		}
	}
}
