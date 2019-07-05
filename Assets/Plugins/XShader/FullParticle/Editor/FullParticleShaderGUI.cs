using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

internal class FullParticleShaderGUI : ShaderGUI {
	public enum BlendPreset {
		Alpha_Blend = 0,
		Add = 1,
		Multiply = 2,
		AddRev = 3,
		PreMulBlend = 4,
		Custom = 50
	}

	private MaterialProperty blendPresetProperty;
	private MaterialProperty blendModeSrcProperty;
	private MaterialProperty blendModeDstProperty;
	private MaterialProperty blendOpProperty;

	private MaterialProperty preMulAlphaProperty;
	private MaterialProperty revVertColorProperty;
	private MaterialProperty revTexColorProperty;
	private MaterialProperty alphaFromLengthProperty;
	private MaterialProperty alphaIntensityProperty;

	private MaterialProperty mainTexProperty;

	private MaterialProperty intensityProperty;
	private MaterialProperty colorPorperty;
	private MaterialProperty cullPorperty;
	private MaterialProperty zTestProperty;
	private MaterialProperty uvMovePorperty;
	private MaterialProperty fogEnablePorperty;
	private MaterialProperty fogColorEnablePorperty;
	private MaterialProperty fogColorPorperty;
	private MaterialProperty colorMaskPorperty;
	private MaterialProperty InvFadePorperty;

	private MaterialProperty stencilRefPorperty;
	private MaterialProperty stencilCompPorperty;
	private MaterialProperty stencilOpPorperty;
	private MaterialProperty stencilWriteMaskPorperty;
	private MaterialProperty stencilReadMaskPorperty;


	BlendPreset blendPreset;
	BlendMode blendModeSrc;
	BlendMode blendModeDst;
	BlendOp blendOp;

	private bool showOther = false;
	private Vector2 uvMove;
	bool zTest;
	CullMode cullMode;
	bool fogEnable;
	bool fogColorEnable = false;
	private ColorWriteMask colorWriteMask = ColorWriteMask.Red | ColorWriteMask.Green | ColorWriteMask.Blue;

	CompareFunction stencilComp;
	StencilOp stencilOp;


	public virtual void FindProperties(MaterialProperty[] props) {
		this.blendPresetProperty = FindProperty("_BlendPreset", props);
		this.blendModeSrcProperty = FindProperty("_BlendModeSrc", props);
		this.blendModeDstProperty = FindProperty("_BlendModeDst", props);
		this.blendOpProperty = FindProperty("_BlendOp", props);

		this.preMulAlphaProperty = FindProperty("_PreMulAlpha", props);
		this.revVertColorProperty = FindProperty("_RevVertColor", props);
		this.revTexColorProperty = FindProperty("_RevTexColor", props);
		this.alphaFromLengthProperty = FindProperty("_AlphaFromLength", props);
		this.alphaIntensityProperty = FindProperty("_AlphaIntensity", props);

		this.mainTexProperty = FindProperty("_MainTex", props);
		this.zTestProperty = FindProperty("_ZTest", props);
		this.intensityProperty = FindProperty("_Intensity", props);
		this.colorPorperty = FindProperty("_Color", props);
		this.cullPorperty = FindProperty("_Cull", props);
		this.uvMovePorperty = FindProperty("_UVMove", props);
		this.fogEnablePorperty = FindProperty("_FogEnable", props);
		this.fogColorEnablePorperty = FindProperty("_FogColorEnable", props);
		this.fogColorPorperty = FindProperty("_FogColor", props);
		this.colorMaskPorperty = FindProperty("_ColorMask", props);
		this.InvFadePorperty = FindProperty("_InvFade", props);

		this.stencilRefPorperty = FindProperty("_StencilRef", props);
		this.stencilCompPorperty = FindProperty("_StencilComp", props);
		this.stencilOpPorperty = FindProperty("_StencilOp", props);
		this.stencilReadMaskPorperty = FindProperty("_StencilWriteMask", props);
		this.stencilWriteMaskPorperty = FindProperty("_StencilReadMask", props);
	}

	public virtual void SetupBlendMode() {
		blendPreset = (BlendPreset)this.blendPresetProperty.floatValue;
		blendModeSrc = (BlendMode)this.blendModeSrcProperty.floatValue;
		blendModeDst = (BlendMode)this.blendModeDstProperty.floatValue;
		blendOp = (BlendOp)this.blendOpProperty.floatValue;
		blendPreset = (BlendPreset)EditorGUILayout.EnumPopup("Blend Mode", blendPreset);
		bool disableCustomBlend = true;
		switch (blendPreset) {
			case BlendPreset.Alpha_Blend:
				blendModeSrc = BlendMode.SrcAlpha;
				blendModeDst = BlendMode.OneMinusSrcAlpha;
				blendOp = BlendOp.Add;
				break;
			case BlendPreset.Add:
				blendModeSrc = BlendMode.SrcAlpha;
				blendModeDst = BlendMode.One;
				blendOp = BlendOp.Add;
				break;
			case BlendPreset.AddRev:
				blendModeSrc = BlendMode.SrcAlpha;
				blendModeDst = BlendMode.One;
				blendOp = BlendOp.ReverseSubtract;
				break;
			case BlendPreset.Multiply:
				blendModeSrc = BlendMode.Zero;
				blendModeDst = BlendMode.SrcColor;
				blendOp = BlendOp.Add;
				break;
			case BlendPreset.PreMulBlend:
				blendModeSrc = BlendMode.One;
				blendModeDst = BlendMode.OneMinusSrcAlpha;
				blendOp = BlendOp.Add;
				break;
			case BlendPreset.Custom:
				disableCustomBlend = false;
				break;
		}
		EditorGUI.BeginDisabledGroup(disableCustomBlend);
		blendModeSrc = (BlendMode)EditorGUILayout.EnumPopup("Src", blendModeSrc);
		blendModeDst = (BlendMode)EditorGUILayout.EnumPopup("Dest", blendModeDst);
		blendOp = (BlendOp)EditorGUILayout.EnumPopup("Op", blendOp);
		EditorGUI.EndDisabledGroup();
	}

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props) {
		FindProperties(props);

		materialEditor.TextureProperty(this.mainTexProperty, "MainTex");

		EditorGUI.BeginChangeCheck();
		SetupBlendMode();

		this.preMulAlphaProperty.floatValue = EditorGUILayout.Toggle("PreMulAlpha", this.preMulAlphaProperty.floatValue != 0) ? 1 : 0;
		if (this.preMulAlphaProperty.floatValue != 1 && blendPreset == BlendPreset.PreMulBlend) {
			EditorGUILayout.HelpBox("PreMulBlend模式下建议开启PreMulAlpha，否则不会淡出，并且不支持雾", MessageType.Warning);
		}

		this.revVertColorProperty.floatValue = EditorGUILayout.Toggle("RevVertColor", this.revVertColorProperty.floatValue != 0) ? 1 : 0;
		if (this.revVertColorProperty.floatValue != 1&&blendOp==BlendOp.ReverseSubtract) {
			EditorGUILayout.HelpBox("ReverseSubtract模式下建议开启RevVertColor", MessageType.Warning);
		}

		this.revTexColorProperty.floatValue = EditorGUILayout.Toggle("RevTexColor", this.revTexColorProperty.floatValue != 0) ? 1 : 0;
		this.alphaFromLengthProperty.floatValue = EditorGUILayout.Toggle("AlphaFromLength", this.alphaFromLengthProperty.floatValue != 0) ? 1 : 0;
		if (this.alphaFromLengthProperty.floatValue == 1) {
			materialEditor.RangeProperty(alphaIntensityProperty, "AlphaIntensity");
		}


		materialEditor.RangeProperty(this.intensityProperty, "Intensity");
		materialEditor.ColorProperty(this.colorPorperty, "Color");

//		showOther = EditorGUILayout.Foldout(showOther, "Others");
//		if (showOther) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			uvMove = new Vector2(this.uvMovePorperty.vectorValue.x, this.uvMovePorperty.vectorValue.y);
			uvMove = EditorGUILayout.Vector2Field("UVMove", uvMove);

			zTest = this.zTestProperty.floatValue == 0f ? false : true;
			zTest = EditorGUILayout.Toggle("ZTest", zTest);

			cullMode = (CullMode)this.cullPorperty.floatValue;
			cullMode = (CullMode)EditorGUILayout.EnumPopup("Cull", cullMode);

			fogEnable = this.fogEnablePorperty.floatValue == 1 ? true : false;
			fogEnable = EditorGUILayout.Toggle("Fog", fogEnable);
			if (fogEnable) {
				fogColorEnable = this.fogColorEnablePorperty.floatValue == 1 ? true : false;
				fogColorEnable = EditorGUILayout.Toggle("FogColor", fogColorEnable);
				if (fogColorEnable) {
					materialEditor.ColorProperty(this.fogColorPorperty, "FogColor");
				}
			}


			colorWriteMask = (ColorWriteMask)EditorGUILayout.EnumMaskPopup(new GUIContent("ColorMask"), colorWriteMask);
			materialEditor.RangeProperty(this.InvFadePorperty, "Soft Particles Factor");

			stencilComp = (CompareFunction)this.stencilCompPorperty.floatValue;
			stencilComp = (CompareFunction)EditorGUILayout.EnumPopup("_StencilComp", stencilComp);
			if (stencilComp != CompareFunction.Disabled) {//disabled will turn off stencil
				materialEditor.FloatProperty(this.stencilRefPorperty, "_StencilRef");
				stencilOp = (StencilOp)EditorGUILayout.EnumPopup("_StencilOp", stencilOp);
				materialEditor.FloatProperty(stencilWriteMaskPorperty, "Stencil Write");
				materialEditor.FloatProperty(stencilReadMaskPorperty, "Stencil Read");
			}
//		}


		if (EditorGUI.EndChangeCheck()) {
			materialEditor.RegisterPropertyChangeUndo("Change Particle Materail");
			ApplyChange(materialEditor);
		}
	}

	public virtual void ApplyChange(MaterialEditor materialEditor) {
		this.blendPresetProperty.floatValue = (float)blendPreset;
		this.blendModeSrcProperty.floatValue = (float)blendModeSrc;
		this.blendModeDstProperty.floatValue = (float)blendModeDst;
		this.blendOpProperty.floatValue = (float)blendOp;

		this.zTestProperty.floatValue = zTest ? 2 : 0;

		foreach (var obj in this.blendPresetProperty.targets) {
			this.blendModeSrcProperty.floatValue = (float)blendModeSrc;
			this.blendModeDstProperty.floatValue = (float)blendModeDst;
			this.blendOpProperty.floatValue = (float)blendOp;

			this.uvMovePorperty.vectorValue = new Vector4(uvMove.x, uvMove.y, 0, 0);
			this.zTestProperty.floatValue = zTest ? 2 : 0;
			this.cullPorperty.floatValue = (float)cullMode;
			this.fogEnablePorperty.floatValue = fogEnable ? 1 : 0;
			this.fogColorEnablePorperty.floatValue = fogColorEnable ? 1 : 0;
			this.colorMaskPorperty.floatValue = (float)colorWriteMask;

			//stencil
			this.stencilCompPorperty.floatValue = (float)this.stencilComp;
			this.stencilOpPorperty.floatValue = (float)this.stencilOp;

			var mat = (Material)obj;
			SetKeyword(mat, "PreMulAlpha_On", this.preMulAlphaProperty.floatValue == 1f);
			SetKeyword(mat, "RevVertColor_On", this.revVertColorProperty.floatValue == 1f);
			SetKeyword(mat, "RevTexColor_On", this.revTexColorProperty.floatValue == 1f);
			SetKeyword(mat, "AlphaFromLength_On", this.alphaFromLengthProperty.floatValue == 1f);
			SetKeyword(mat, "Intensity_On", this.intensityProperty.floatValue != 1f);
			SetKeyword(mat, "Color_On", this.colorPorperty.colorValue != Color.white);
			var uvMoveValue = this.uvMovePorperty.vectorValue;
			SetKeyword(mat, "UVMove_On", uvMoveValue.x != 0 || uvMoveValue.y != 0);
			SetKeyword(mat, "Fog_On", fogEnable);
			SetKeyword(mat, "FogColor_On", fogColorEnable);
		}
	}

	public static void SetKeyword(Material m, string keyword, bool state) {
		if (state) {
			m.EnableKeyword(keyword);
		} else {
			m.DisableKeyword(keyword);
		}
	}
}

