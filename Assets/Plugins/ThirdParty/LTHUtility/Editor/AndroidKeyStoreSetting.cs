using UnityEngine;
using UnityEditor;

namespace LTHUtility {


	[InitializeOnLoad]
	public class AndroidKeyStoreSetting : EditorWindow {
		private const string KEY_STORE_NAME_PREFS = "KEY_STORE_NAME_PREFS";
		private const string KEY_STORE_PASS_PREFS = "KEY_STORE_PASS_PREFS";
		private const string KEY_ALIAS_NAME_PREFS = "KEY_ALIAS_NAME_PREFS";
		private const string KEY_ALIAS_PASS_PREFS = "KEY_ALIAS_PASS_PREFS";

		public static string keystoreName;
		public static string keystorePass;

		public static string keyaliasName;
		public static string keyaliasPass;

		[MenuItem("LTH/Utility/配置安卓密钥")]
		public static void ShowWindow() {
			AndroidKeyStoreSetting win = EditorWindow.CreateInstance<AndroidKeyStoreSetting>();
			win.Show();
		}

		static AndroidKeyStoreSetting() {
			keystoreName = EditorPrefs.GetString(KEY_STORE_NAME_PREFS, Application.dataPath + "/../MogooMobile.keystore");
			keystorePass = EditorPrefs.GetString(KEY_STORE_PASS_PREFS, "mogoo@mogoo123");
			keyaliasName = EditorPrefs.GetString(KEY_ALIAS_NAME_PREFS, "mogoomobile");
			keyaliasPass = EditorPrefs.GetString(KEY_ALIAS_PASS_PREFS, "mogoo@mogoo123");
		}



		private static bool alreadySign = false;
		//[UnityEditor.Callbacks.PostProcessScene]
		static void OnBuild() {
			if (!BuildPipeline.isBuildingPlayer) {
				return;
			}
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
				if (!alreadySign) {
					Save();
					alreadySign = true;
				}
			}
		}

		[UnityEditor.Callbacks.PostProcessBuild]
		static void OnBuildOver(UnityEditor.BuildTarget buildTarget, string output) {
			alreadySign = false;
		}

		static void Save() {
			EditorPrefs.SetString(KEY_STORE_NAME_PREFS, keystoreName);
			EditorPrefs.SetString(KEY_STORE_PASS_PREFS, keystorePass);
			EditorPrefs.SetString(KEY_ALIAS_NAME_PREFS, keyaliasName);
			EditorPrefs.SetString(KEY_ALIAS_PASS_PREFS, keyaliasPass);

			PlayerSettings.Android.keystoreName = keystoreName;
			PlayerSettings.Android.keystorePass = keystorePass;
			PlayerSettings.Android.keyaliasName = keyaliasName;
			PlayerSettings.Android.keyaliasPass = keyaliasPass;
			Debug.Log(string.Format("设置安卓打包密钥：{0},密码：{1}，alias：{2}，alias密码：{3}",
				PlayerSettings.Android.keystoreName,
				PlayerSettings.Android.keystorePass,
				PlayerSettings.Android.keyaliasName,
				PlayerSettings.Android.keyaliasPass)
				);
		}


		void OnGUI() {
			GUILayout.BeginVertical();
			if (GUILayout.Button("选择Key文件")) {
				keystoreName = EditorUtility.OpenFilePanel("选择Key", Application.dataPath + "/../", "keystore");
				Save();
			}
			keystoreName = EditorGUILayout.TextField(new GUIContent("Key文件"), keystoreName);
			keystorePass = EditorGUILayout.TextField(new GUIContent("Key密码"), keystorePass);
			keyaliasName = EditorGUILayout.TextField(new GUIContent("Alias名称"), keyaliasName);
			keyaliasPass = EditorGUILayout.TextField(new GUIContent("Alias密码"), keyaliasPass);

			if (GUILayout.Button("Save")) {
				Save();
			}

			GUILayout.EndVertical();

		}
	}
}