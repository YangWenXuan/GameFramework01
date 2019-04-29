using UnityEngine;
using UnityEngine.UI;

public static class UGUIGrey {

	public static Material UI_Default_Grey_Mat;

	[RuntimeInitializeOnLoadMethod]
	static void Init() {
		var shader = Shader.Find("UI/Default-Grey");
		if (shader == null) {
			Debug.LogError("没有找到UI/Default-Grey，你是否移除了它?");
		}
		UI_Default_Grey_Mat = new Material(shader);
		UI_Default_Grey_Mat.hideFlags = HideFlags.DontSave;
	}

	/// <summary>
	/// 将UI元素置为置灰状态或者关闭置灰状态
	/// </summary>
	/// <param name="image"></param>
	/// <param name="grey"></param>
	/// <param name="originMaterial"></param>
	public static void SetGreyMaterail(this Graphic image, bool grey, Material originMaterial = null) {
		image.material = grey ? UI_Default_Grey_Mat : originMaterial;
	}

	/// <summary>
	/// Image是否处于置灰状态
	/// </summary>
	/// <param name="image"></param>
	/// <returns></returns>
	public static bool IsGrey(this Graphic image) {
		return image.material == UI_Default_Grey_Mat;
	}

	public static void SetBtnDisableGrey(this Button btn, bool enable, Color? labelColor=null) {
		btn.interactable = enable;
		foreach (var image in btn.GetComponentsInChildren<Image>()) {
			image.SetGreyMaterail(!enable);
		}
	}

}

