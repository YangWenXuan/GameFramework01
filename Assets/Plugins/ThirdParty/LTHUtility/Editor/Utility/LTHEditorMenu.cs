using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LTHEditorMenu : MonoBehaviour {

//	[MenuItem("LTH/Utility/保存资源 &3")]
//	static void SaveAsset() {
//		EditorApplication.SaveAssets();
//	}

	[MenuItem("LTH/Utility/运行（退出） &1")]
	static void TogglePlayMode() {
		EditorApplication.ExecuteMenuItem("Edit/Play");
	}

	[MenuItem("LTH/Utility/暂停 &2")]
	static void Pause() {
		EditorApplication.ExecuteMenuItem("Edit/Pause");
	}

	[MenuItem("LTH/Utility/对选中的物体计数")]
	static void Count() {
		Debug.Log(Selection.objects.Length);
	}

	[MenuItem("LTH/Utility/对选中的物体排列")]
	static void Arrange() {
		int i = 0;
		foreach (var t in Selection.transforms) {
			t.SetPositionX(i*2);
			i++;
		}
	}

	[MenuItem("LTH/Utility/显示隐藏物体")]
	static void ShowHideObjects() {
		Transform[] ts = FindObjectsOfType<Transform>();
		foreach (var v in ts) {
			if (v.gameObject.hideFlags != HideFlags.None) {
				v.gameObject.hideFlags = HideFlags.None;
			}
		}
	}

//	[MenuItem("Assets/移除png的A通道")]
//	static void RemoveAlphaChanel() {
//		Texture2D tex = Selection.activeObject as Texture2D;
//		if (!tex) {
//			EditorUtility.DisplayDialog("error", "请选择一张图片", "ok");
//			return;
//		}
//		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
//		if (!path.EndsWith("png") && !path.EndsWith("PNG")) {
//			EditorUtility.DisplayDialog("error", "图片不是png或PNG:" + path, "ok");
//			return;
//		}
//		TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
//		TextureImporterFormat originFormat = importer.textureFormat;
//		bool haveAlpha = importer.DoesSourceTextureHaveAlpha();
//		if (!haveAlpha) {
//			Debug.Log("已经没有alpha");
//			return;
//		}
//		bool originIsReadAble = importer.isReadable;
//		importer.isReadable = true;
//		importer.textureFormat = TextureImporterFormat.ARGB32;
//		AssetDatabase.ImportAsset(importer.assetPath);
//
//		Color32[] pix = tex.GetPixels32();
//		for (int i = 0; i < pix.Length; i++) {
//			pix[i].a = 0;
//		}
//
//
//		Texture2D newTex = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
//		newTex.SetPixels32(pix);
//
//		var bytes = newTex.EncodeToPNG();
//		File.WriteAllBytes(path, bytes);
//
//
//		AssetDatabase.Refresh();
//		importer.isReadable = originIsReadAble;
//		importer.textureFormat = originFormat;
//		AssetDatabase.ImportAsset(importer.assetPath);
//
//
//	}

	//	[MenuItem("Assets/创建一张用R表示A的PNG")]
	//	private static void CreateAlphaPNG() {
	//		Texture2D tex = Selection.activeObject as Texture2D;
	//		if (!tex) {
	//			EditorUtility.DisplayDialog("error", "请选择一张图片", "ok");
	//			return;
	//		}
	//		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
	//		if (!path.EndsWith("png") && !path.EndsWith("PNG")) {
	//			EditorUtility.DisplayDialog("error", "图片不是png或PNG:" + path, "ok");
	//			return;
	//		}
	//		TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
	//		TextureImporterFormat originFormat = importer.textureFormat;
	//		bool haveAlpha = importer.DoesSourceTextureHaveAlpha();
	//		if (!haveAlpha) {
	//			Debug.Log("已经没有alpha");
	//			return;
	//		}
	//		bool originIsReadAble = importer.isReadable;
	//		importer.isReadable = true;
	//		importer.textureFormat = TextureImporterFormat.ARGB32;
	//		AssetDatabase.ImportAsset(importer.assetPath);
	//
	//		Color32[] pix = tex.GetPixels32();
	//		Color32[] newPix = new Color32[pix.Length];
	//		for (int i = 0; i < pix.Length; i++) {
	//			newPix[i] = new Color32(pix[i].a, 0, 0, 0);
	//		}
	//
	//		Texture2D newTex = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
	//		newTex.SetPixels32(newPix);
	//
	//		var bytes = newTex.EncodeToPNG();
	//		path += "-alpha.png";
	//		File.WriteAllBytes(path, bytes);
	//
	//		AssetDatabase.Refresh();
	//		importer.isReadable = originIsReadAble;
	//		importer.textureFormat = originFormat;
	//		AssetDatabase.ImportAsset(importer.assetPath);
	//	}

#if UNITY_EDITOR_WIN
	//	[MenuItem("Visual Studio Tools/Clean and ReGenerate")]
	//	private static void CleanUnityVSProject() {
	//
	//		var files = Directory.GetFiles(Application.dataPath + "/../");
	//		foreach (var file in files) {
	//			if (file.Contains("UnityVS")) {
	//				if (file.EndsWith(".csproj") || file.EndsWith(".sln") || file.EndsWith(".suo")) {
	//					File.Delete(file);
	//				}
	//			}
	//		}
	//		EditorApplication.ExecuteMenuItem("Visual Studio Tools/Generate Project Files");
	//	}
#endif

	//	[MenuItem("LTH/Utility/InspectorSetToInternalDebug")]
	//	private static void InspectorSetToInternalDebug() {
	//		Type inspectorType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
	//		var ins = Resources.FindObjectsOfTypeAll(inspectorType);
	//
	//		MethodInfo setDebgInternal = inspectorType.GetMethod("SetDebugInternal", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
	//		inspectorType.GetMethods();
	//		inspectorType.GetMembers(BindingFlags.NonPublic);
	//		inspectorType.GetProperties(BindingFlags.NonPublic);
	//
	//
	//		foreach (var i in ins) {
	//			setDebgInternal.Invoke(i, new object[0]);
	//
	//		}
	//	}

	//	[MenuItem("LTH/Utility/打开或关闭所有灯光 &l")]
	//	public static void TogleAllLits() {
	//		var lits = FindObjectsOfType<Light>();
	//		if (lits == null || lits.Length == 0) {
	//			Debug.Log("没有找到灯光");
	//			return;
	//		}
	//		bool enable = !lits[0].enabled;
	//		foreach (var lit in lits) {
	//			lit.enabled = enable;
	//		}
	//		Debug.Log("所有灯光目前为  " + (enable ? "开启" : "关闭"));
	//
	//	}



	//	[MenuItem("Assets/选择相关材质")]
	//	private static void ChooseReferenceMat() {
	//		GameObject[] gs = Selection.gameObjects;
	//		if (gs == null || gs.Length == 0) {
	//			EditorUtility.DisplayDialog("", "至少选择一个 prefab", "ok");
	//			return;
	//		}
	//		var deps = EditorUtility.CollectDependencies(gs);
	//		List<Object> mats = new List<Object>();
	//		foreach (var dep in deps) {
	//			if (dep.GetType() == typeof(Material)) {
	//				mats.Add(dep);
	//			}
	//		}
	//		Selection.objects = mats.ToArray();
	//	}



}
