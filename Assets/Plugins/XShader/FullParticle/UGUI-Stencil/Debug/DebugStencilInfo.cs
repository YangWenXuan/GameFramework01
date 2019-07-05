using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DebugStencilInfo : MonoBehaviour {

	public Material mat;
	public CompareFunction comp;
	public float compF;
	public string matName;
	public float stencil;
	public StencilOp pass;
	public float passF;
	public float w;
	public float r;


	public int stencil_Calc;

	[ContextMenu("OnValidate")]
	void OnValidate() {
		if (GetComponent<MaskableGraphic>() != null) {
			mat = GetComponent<MaskableGraphic>().materialForRendering;
			matName = mat.name;
			stencil = mat.GetFloat("_Stencil");
			comp = (CompareFunction)mat.GetFloat("_StencilComp");
			compF = (float)comp;
			pass = (StencilOp)mat.GetFloat("_StencilOp");
			passF = (float)pass;
			w = mat.GetFloat("_StencilWriteMask");
			r = mat.GetFloat("_StencilReadMask");
			GetComponent<MaskableGraphic>().SetAllDirty();
			GetComponent<MaskableGraphic>().SetMaterialDirty();
		}

		stencil_Calc = MaskUtilities.GetStencilDepth(this.transform, MaskUtilities.FindRootSortOverrideCanvas(this.transform));
	}

	void Update() {
		OnValidate();
	}

}
